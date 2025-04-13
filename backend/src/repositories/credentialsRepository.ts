import { AppDataSource } from "../libs/data-source";
import { Credential } from "../libs/entities/Credential";
import { Player } from "../libs/entities/Player";

const credentialsRepo = AppDataSource.getRepository(Credential);

export interface NewCred {
  player_id: string;
  hashedEmail: string;
  hashedPassword: string;
}

export async function saveCredential(newCred: NewCred): Promise<Credential> {
  try {
    const credential = new Credential();
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
