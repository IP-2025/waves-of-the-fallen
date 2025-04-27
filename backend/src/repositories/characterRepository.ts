import { AppDataSource } from '../libs/data-source';
import { Character } from '../libs/entities/Character';

async function deleteAll() {
  await AppDataSource.getRepository(Character).clear();
}

export async function insertAllCharacters(chars: Character[]): Promise<void> {
  await deleteAll();
  await AppDataSource.getRepository(Character).save(chars);
}
export async function getAllCharactersRepo(): Promise<Character[]> {
  return await AppDataSource.getRepository(Character).find();
}
export async function getAllUnlockedCharactersRepo(playerId:string): Promise<Character[]> {
  return await AppDataSource.getRepository(Character).find();
}
