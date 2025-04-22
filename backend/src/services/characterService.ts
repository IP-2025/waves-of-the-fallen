import { Character } from '../libs/entities/Character';
import { insertAllCharacters,getAllCharactersRepo } from '../repositories/characterRepository';

export async function innitAllCharacters(): Promise<void> {
  await insertAllCharacters();
}

export async function getAllCharacters(): Promise<void> {
  await getAllCharactersRepo();
}
