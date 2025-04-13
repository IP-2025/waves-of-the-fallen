import { AppDataSource } from '../libs/data-source';
import { Player } from '../libs/entities/Player';
import logger from '../logger/logger';
import { InternalServerError } from '../errors';

const playersRepo = AppDataSource.getRepository(Player);

export interface NewPlayer {
  player_id: string;
  username: string;
}

export async function createNewPlayer(user: NewPlayer) {
  try {
    const player = new Player();
    player.player_id = user.player_id;
    player.username = user.username;

    const createdUser = await playersRepo.save(player);
    return createdUser;
  } catch (error) {
    logger.error('Error inserting new user: ', error);
    throw new InternalServerError('Error inserting new user');
  }
}
