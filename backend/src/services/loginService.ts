import { getPwdByMail, getPlayerIdFromCredential } from 'repositories';
import { InternalServerError, NotFoundError, UnauthorizedError } from 'errors';
import bcrypt from 'bcrypt';
import { generateToken } from 'auth/jwt';

export async function pwdCheck(email: string, password: string): Promise<string> {
  try {
    const credential = await getPwdByMail(email);
    if (!credential) {
      throw new UnauthorizedError('Credential not found for the given email.');
    }
    const isMatch = await bcrypt.compare(password, credential.password);
    if (!isMatch) {
      throw new UnauthorizedError('Invalid password.');
    }

    const player_id = await getPlayerIdFromCredential(credential.id);
    if (!player_id) {
      throw new UnauthorizedError('Player ID not found for the given credential.');
    }

    // creating jwt
    const token = generateToken(player_id);
    return token;
  } catch (error) {
    if (error instanceof UnauthorizedError) {
      throw error;
    }
    if (error instanceof NotFoundError) {
      throw new UnauthorizedError('Invalid email or password');
    }
    throw new InternalServerError('Internal server error');
  }
}
