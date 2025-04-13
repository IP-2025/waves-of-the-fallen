import { getPrismaClient } from '../libs';
import logger from '../logger/logger';
import { InternalServerError } from '../errors';

const prisma = getPrismaClient();

export interface NewCred {
  player_id: string;
  username: string,
}

export async function createNewUser(user: NewCred) {
  try {
    const createdUser = await prisma.user.create({
      data: {
        player_id: user.player_id,
        username: user.username,
      },
    });
    return createdUser;
  } catch (error) {
    logger.error('Error inserting new user: ', error);
    throw new InternalServerError('Error inserting new user');
  }
}
