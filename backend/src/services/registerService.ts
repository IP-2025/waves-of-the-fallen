import bcrypt from 'bcrypt';
import { insertNewCred } from '../repositories/credentialsRepository';
import { InternalServerError } from '../errors';
import { v4 as uuidv4 } from 'uuid';
import logger from '../logger/logger';
import { createNewUser } from '../repositories/usersRepository';

export async function registerUser(username: string, password: string, mail: string): Promise<void> {
  try {
    // Hash the password and email
    const hashedPassword = await bcrypt.hash(password, 10);
    const hashedMail = await bcrypt.hash(mail, 10);
    const playerId = uuidv4();

    const user = await insertNewCred({ player_id: playerId, email: hashedMail, password: hashedPassword });

    await createNewUser({ player_id: user.player_id, username: username})
  } catch (error) {
    logger.error('Error registering user: ', error);

    throw new InternalServerError('Failed to register user. Please try again later.');
  }
}