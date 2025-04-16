import { getPwdByMail, getPlayerIdFromCredential } from '../repositories/credentialsRepository';
import { NotFoundError } from '../errors';
import bcrypt from 'bcrypt';
import { generateToken } from '../auth/jwt';

export async function pwdCheck(email: string, password: string): Promise<string> {
  const credential = await getPwdByMail(email);
  if (!credential) {
    throw new NotFoundError('Credential not found for the given email.');
  }
  const isMatch = await bcrypt.compare(password, credential.password);
  if (!isMatch) {
    throw new NotFoundError('Invalid password.');
  }

  const player_id = await getPlayerIdFromCredential(credential.id);
    if (!player_id) {
        throw new NotFoundError('Player ID not found for the given credential.');
    }

  // creating jwt
  const token = generateToken(player_id);
  return token;
}
