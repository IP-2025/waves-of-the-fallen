import { Character } from '../libs/entities/Character';
import { insertAllCharacters, getAllCharactersRepo } from '../repositories/characterRepository';
import fs from 'fs';

export async function innitAllCharacters(chars?: Character[]): Promise<void> {
  const characters = chars ?? readCharacters();
  const allCharacters = await getAllCharactersRepo() || [];

  // 1) If DB has no rows yet, just insert all characters
  if (allCharacters.length === 0) {
    await insertAllCharacters(characters);
    return;
  }

  // 2) If lengths differ, we know they're not equal, so insert
  if (allCharacters.length !== characters.length) {
    await insertAllCharacters(characters);
    return;
  }

  // 3) Now safe to compare item‑by‑item
  const equal = characters.every((item, i) => {
    const src = allCharacters[i]!;
    const keys = Object.keys(item) as (keyof Character)[];
    return (
        keys.length === Object.keys(src).length &&
        keys.every((key) => src[key] === item[key])
    );
  });

  if (!equal) {
    await insertAllCharacters(characters);
  }
}

export async function getAllCharacters(): Promise<Character[]> {
  const chars = await getAllCharactersRepo();
  return chars;
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
//TODO Implement Stuff
function getAllUnlockedCharacters() {

}