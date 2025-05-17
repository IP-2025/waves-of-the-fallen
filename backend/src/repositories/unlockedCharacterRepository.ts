import {AppDataSource} from "../libs/data-source";
import {UnlockedCharacter} from "../libs/entities/UnlockedCharacter";
import {InternalServerError} from "../errors";
import {Player} from "../libs/entities/Player";
import {Character} from "../libs/entities/Character";

const unlockCharRepo = AppDataSource.getRepository(UnlockedCharacter);
const playerRepo = AppDataSource.getRepository(Player);

export async function unlockCharacter(playerId: string, characterId: number) {
    const unlockedCharacter = new UnlockedCharacter();
    unlockedCharacter.level = 1;
    unlockedCharacter.character_id = characterId;

    const player = await playerRepo.findOneBy({ player_id: playerId });
    if (!player) {
        throw new InternalServerError(`Player with ID ${playerId} not found`);
    }

    unlockedCharacter.player = player;

    await unlockCharRepo.save(unlockedCharacter);
}

export async function levelUpCharacter(playerId: string, characterId: number) {
    const unlockedCharacter = await unlockCharRepo.findOne({
        where: {
            player: { player_id: playerId },
            character_id: characterId
        }
    });

    if (unlockedCharacter) {
        unlockedCharacter.level += 1;
        await unlockCharRepo.save(unlockedCharacter);
    } else {
        throw new InternalServerError(`Character with ID ${characterId} not found for player ${playerId}`);
    }
}
