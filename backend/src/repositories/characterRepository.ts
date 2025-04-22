import * as fs from 'fs';
import { AppDataSource } from '../libs/data-source';
import { Character } from '../libs/entities/Character';

export async function insertAllCharacters(): Promise<void> {
  const jsonData = JSON.parse(fs.readFileSync('assets/characters/innitCharacters.json', 'utf-8'));
  for (const character of jsonData.characters) {
    const newCharacter = new Character();
    newCharacter.characterId = character.characterId;
    newCharacter.name = character.name;
    newCharacter.speed = character.speed;
    newCharacter.health = character.health;
    newCharacter.dexterity = character.dexterity;
    newCharacter.intelligence = character.intelligence;
    await AppDataSource.getRepository(Character).save(newCharacter);
  }
}
export async function getAllCharactersRepo(): Promise<Character[]> {
  return await AppDataSource.getRepository(Character).find();
}
