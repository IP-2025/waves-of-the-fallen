import { AppDataSource } from "../libs/data-source";
import { Credential } from "../libs/entities/Credential";
import { Player } from "../libs/entities/Player";
import { v4 as uuidv4 } from 'uuid';

const credentialsRepo = AppDataSource.getRepository(Credential);

export interface NewCred {
  player_id: string;
  hashedEmail: string;
  hashedPassword: string;
}

export async function saveCredential(newCred: NewCred): Promise<Credential> {
  try {
    // Validate required fields
    if (!newCred.player_id || !newCred.hashedEmail || !newCred.hashedPassword) {
      throw new Error('Missing required fields');
    }

    const credential = new Credential();
    credential.id = uuidv4();
    credential.email = newCred.hashedEmail;
    credential.password = newCred.hashedPassword;

    // Find the player by player_id
    const playerRepo = AppDataSource.getRepository(Player);
    const player = await playerRepo.findOneBy({ player_id: newCred.player_id });

    if (!player) {
      throw new Error('Player not found');
    }

    credential.player = player;

    const savedCredential = await credentialsRepo.save(credential);
    return savedCredential;
  } catch (error) {
    if (error instanceof Error && 'code' in error && error.code === '23505') {
      throw new Error('Email already exists');
    }
    throw error;
  }
}
