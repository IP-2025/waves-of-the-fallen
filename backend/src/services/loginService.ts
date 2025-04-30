import { getPwdByMail, getPlayerIdFromCredential } from '../repositories/credentialsRepository';
import { InternalServerError, UnauthorizedError } from '../errors';
import bcrypt from 'bcrypt';
import { generateToken } from '../auth/jwt';

export async function pwdCheck(email: string, password: string): Promise<string> {
  try {
    const credential = await getPwdByMail(email);

    const isMatch = await bcrypt.compare(password, credential.password);
    if (!isMatch) {
      throw new UnauthorizedError('Wrong email or password.');
    }

    const token = generateToken(credential.player.player_id);
    return token;
  } catch (error) {
    if (error instanceof UnauthorizedError) {
      throw error;
    }

    console.error('Authentication error:', error);

    throw new InternalServerError('An error occurred during authentication.');
  }
}
