import bcrypt from 'bcrypt';
import { InternalServerError } from '../errors';
import { v4 as uuidv4 } from 'uuid';
import logger from '../logger/logger';
import { createNewPlayer } from '../repositories/playerRepository';
import { saveCredential } from '../repositories/credentialsRepository';
import { Player } from '../libs/entities/Player';

export async function registerUser(
  username: string,
  password: string,
  mail: string
): Promise<string> {
  try {
    const hashedPassword = await bcrypt.hash(password, 10);
    const playerId = uuidv4();

    const createdPlayer = await createNewPlayer({
      player_id: playerId,
      username: username,
    });

    await saveCredential({
      player_id: createdPlayer.player_id,
      email: mail,
      hashedPassword: hashedPassword,
    });

    return createdPlayer.player_id; // <-- return the ID
  } catch (error) {
    logger.error('Error registering user: ', error);
    throw new InternalServerError('Failed to register user. Please try again later.');
  }
}

