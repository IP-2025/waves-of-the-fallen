// repositories/userRepository.ts
import { getPrismaClient } from '../libs';
import { v4 as uuidv4 } from 'uuid';
import logger from '../logger/logger';
import { InternalServerError } from '../errors';
import { Credential } from '../types/databaseEntries';

const prisma = getPrismaClient();

export interface NewCred {
  player_id: string;
  email: string;
  password: string;
}

export async function insertNewCred(user: NewCred): Promise<Credential> {
  try {
    const createdUser = await prisma.credentials.create({
      data: {
        player_id: user.player_id || uuidv4(),
        email: user.email,
        password: user.password,
        created_at: new Date(),
      },
    });
    return createdUser;
  } catch (error) {
    logger.error('Error inserting new user: ', error);
    throw new InternalServerError('Error inserting new user');
  }
}