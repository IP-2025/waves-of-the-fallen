import {AppDataSource} from '../libs/data-source';
import {Credential} from '../libs/entities/Credential';
import {Player} from '../libs/entities/Player';
import {v4 as uuidv4} from 'uuid';
import {BadRequestError, ConflictError, NotFoundError} from '../errors';

const credentialsRepo = AppDataSource.getRepository(Credential);

export interface NewCred {
  player_id: string;
  email: string;
  hashedPassword: string;
}

export async function saveCredential(newCred: NewCred): Promise<Credential> {
  try {
    // Validate required fields
    if (!newCred.player_id || !newCred.email || !newCred.hashedPassword) {
      throw new BadRequestError('Missing required fields');
    }

    const credential = new Credential();
    credential.id = uuidv4();
    credential.email = newCred.email;
    credential.password = newCred.hashedPassword;

    // Find the player by player_id
    const playerRepo = AppDataSource.getRepository(Player);
    const player = await playerRepo.findOneBy({ player_id: newCred.player_id });

    if (!player) {
      throw new NotFoundError('Player not found');
    }

    credential.player = player;

    return await credentialsRepo.save(credential);
  } catch (error) {
    if (error instanceof Error && 'code' in error && error.code === '23505') {
      throw new ConflictError('Email already exists');
    }
    throw error;
  }
}

export async function getPwdByMail(email: string): Promise<Credential> {
  const credential = await credentialsRepo.findOneBy({ email });
  if (!credential) {
    throw new NotFoundError('Credential not found for the given email.');
  }
  return credential;
}

export async function getPlayerIdFromCredential(credentialId: string): Promise<string | null> {
  const credential = await credentialsRepo.findOne({
    where: { id: credentialId },
    relations: ['player'], // Load the related Player entity
  });

  if (!credential || !credential.player) {
    return null; // Handle case where credential or player is not found
  }

  return credential.player.player_id; // Access the player_id
}
