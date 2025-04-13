import bcrypt from 'bcrypt';
import { InternalServerError } from '../errors';
import { v4 as uuidv4 } from 'uuid';
import logger from '../logger/logger';
import { createNewPlayer } from '../repositories/usersRepository';
import { saveCredential } from '../repositories/credentialsRepository';

export async function registerUser(username: string, password: string, mail: string): Promise<void> {
  try {
    // Hash the password and email
    const hashedPassword = await bcrypt.hash(password, 10);
    const hashedMail = await bcrypt.hash(mail, 10);
    const playerId = uuidv4();

    // Create the player
    const createdPlayer = await createNewPlayer({ player_id: playerId, username: username });

    // Save the credential with reference to the created player
    await saveCredential({ player_id: createdPlayer.player_id, hashedEmail: hashedMail, hashedPassword: hashedPassword });
  } catch (error) {
    logger.error('Error registering user: ', error);
    throw new InternalServerError('Failed to register user. Please try again later.');
  }
}
