import { AppDataSource } from 'database/dataSource';
import { Character } from 'database/entities/Character';
import { UnlockedCharacter } from 'database/entities/UnlockedCharacter';
import { Player } from 'database/entities/Player';
import { NotFoundError } from 'errors';

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
export async function getAllUnlockedCharactersRepo(playerId: string): Promise<UnlockedCharacter[]> {
  return await AppDataSource.getRepository(UnlockedCharacter).find({
    where: {
      player: {
        player_id: playerId,
      },
    },
  });
}

export async function removeCharacter(playerId: string, charId: number): Promise<void> {
  const player = await AppDataSource.getRepository(Player).findOneBy({ player_id: playerId });
  if (!player) {
    throw new NotFoundError('Player not found');
  }

  const unlockedCharacter = await AppDataSource.getRepository(UnlockedCharacter).findOne({
    where: {
      character_id: charId,
      player: {
        player_id: playerId,
      },
    },
  });

  if (unlockedCharacter) {
    await AppDataSource.getRepository(UnlockedCharacter).remove(unlockedCharacter);
  }
}

export async function updateCharacter(playerId: string, charId: number, level: number): Promise<void> {
  const player = await AppDataSource.getRepository(Player).findOneBy({ player_id: playerId });
  if (!player) {
    throw new NotFoundError('Player not found');
  }
  const unlockedCharacter = await AppDataSource.getRepository(UnlockedCharacter).findOne({
    where: {
      character_id: charId,
      player: {
        player_id: playerId,
      },
    },
  });

  if (unlockedCharacter) {
    unlockedCharacter.level = level;
    await AppDataSource.getRepository(UnlockedCharacter).save(unlockedCharacter);
  } else {
    // if the character is not found -> user unlocked a new character so we create a new one
    const newUnlockedCharacter = new UnlockedCharacter();
    newUnlockedCharacter.character_id = charId;
    newUnlockedCharacter.level = level;
    newUnlockedCharacter.player = player;
    await AppDataSource.getRepository(UnlockedCharacter).save(newUnlockedCharacter);
  }
}
