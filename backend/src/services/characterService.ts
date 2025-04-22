import { Character } from '../libs/entities/Character';
import { insertAllCharacters } from '../repositories/characterRepository';

export async function innitAllCharacters(): Promise<void> {
  await insertAllCharacters();
}
