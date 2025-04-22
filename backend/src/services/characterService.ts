import { Character } from '../libs/entities/Character';
import { insertAllCharacters, getAllCharactersRepo } from '../repositories/characterRepository';
import fs from 'fs';

export async function innitAllCharacters(): Promise<void> {
  const chars = readCharacters();

  await insertAllCharacters(chars);
}

export async function getAllCharacters(): Promise<void> {
  await getAllCharactersRepo();
}

function readCharacters(): Character[] {
  const jsonData = JSON.parse(fs.readFileSync('assets/characters/innitCharacters.json', 'utf-8'));
  const toInsertList = new Array<Character>();
  for (const character of jsonData.characters) {
    const newCharacter = new Character();
    newCharacter.characterId = character.characterId;
    newCharacter.name = character.name;
    newCharacter.speed = character.speed;
    newCharacter.health = character.health;
    newCharacter.dexterity = character.dexterity;
    newCharacter.intelligence = character.intelligence;
    toInsertList.push(newCharacter);
  }
  return toInsertList;
}
