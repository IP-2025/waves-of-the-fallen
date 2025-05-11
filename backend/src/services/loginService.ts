import {getPlayerIdFromCredential, getPwdByMail} from 'repositories/credentialsRepository';
import {UnauthorizedError} from 'errors';
import bcrypt from 'bcrypt';
import {generateToken} from 'auth/jwt';

export async function pwdCheck(email: string, password: string): Promise<string> {
    const credential = await getPwdByMail(email);
    if (!credential) {
        throw new UnauthorizedError('email or password is incorrect.');
    }
    const isMatch = await bcrypt.compare(password, credential.password);
    if (!isMatch) {
        throw new UnauthorizedError('email or password is incorrect.');
    }

    const player_id = await getPlayerIdFromCredential(credential.id);
    if (!player_id) {
        throw new UnauthorizedError('email or password is incorrect.');
    }

    // creating jwt
    return generateToken(player_id);
}
