import { Character } from 'database/entities/Character';
import {
  getAllCharactersRepo,
  getAllUnlockedCharactersRepo,
  insertAllCharacters, removeCharacter, updateCharacter,
} from 'repositories/characterRepository';
import fs from 'fs';
import { levelUpCharacter, unlockCharacter } from 'repositories/unlockedCharacterRepository';
import { LocalProgress } from 'types/dto';
import {setGoldService} from "./playerService";

export async function innitAllCharacters(chars?: Character[]): Promise<void> {
  const characters = chars ?? readCharacters();
  const allCharacters = (await getAllCharactersRepo()) ?? [];

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
    return keys.length === Object.keys(src).length && keys.every((key) => src[key] === item[key]);
  });

  if (!equal) {
    await insertAllCharacters(characters);
  }
}

export async function getAllCharacters(): Promise<Character[]> {
  return await getAllCharactersRepo();
}

function readCharacters(): Character[] {
  const jsonData = JSON.parse(fs.readFileSync('assets/characters/innitCharacters.json', 'utf-8'));
  const toInsertList = new Array<Character>();
  for (const character of jsonData.characters) {
    const newCharacter = new Character();
    newCharacter.character_id = character.character_id;
    newCharacter.name = character.name;
    newCharacter.speed = character.speed;
    newCharacter.health = character.health;
    newCharacter.dexterity = character.dexterity;
    newCharacter.strength = character.strength
    newCharacter.intelligence = character.intelligence;
    toInsertList.push(newCharacter);
  }
  return toInsertList;
}

export async function getAllUnlockedCharacters(id: any) {
  return await getAllUnlockedCharactersRepo(id);
}

export async function saveCharChanges(playerId: string, charId: number) {
  await unlockCharacter(playerId, charId);
}
export async function levelUpChar(playerId: string, charId: number) {
  await levelUpCharacter(playerId, charId);
}

export async function progressSyncService(playerId: string, body: any) {
  const localProgress = parseLocalProgress(body);
  const gold = body.gold;

  const dbProgress = await getAllUnlockedCharactersRepo(playerId);
  await setGoldService(playerId, gold);

  // Filtere Charaktere, die in der Datenbank sind, aber nicht in localProgress
  const missingInLocalProgress = dbProgress.filter(
    (dbChar: { character_id: number }) =>
      !localProgress.some(localChar => localChar.character_id === dbChar.character_id)
  );

  for (const char of missingInLocalProgress) {
    await removeCharacter(playerId, char.character_id);
  }

  for (const char of localProgress) {
    await updateCharacter(playerId, char.character_id, char.level);
  }
}

const parseLocalProgress = (data: any): LocalProgress => {
  if (!data || !Array.isArray(data.local_progress)) {
    throw new Error('Invalid data format');
  }

  return data.local_progress.map((item: any) => {
    if (typeof item.character_id !== 'number' || typeof item.level !== 'number') {
      throw new Error('Invalid item format');
    }
    return {
      character_id: item.character_id,
      level: item.level,
    };
  });
};
