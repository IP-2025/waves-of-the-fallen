import {AppDataSource} from "../libs/data-source";
import {UnlockedCharacter} from "../libs/entities/UnlockedCharacter";
import {InternalServerError} from "../errors";

const unlockCharRepo = AppDataSource.getRepository(UnlockedCharacter);

export async function unlockCharacter(playerId: string, characterId: number) {
    const unlockedCharacter = new UnlockedCharacter();
    unlockedCharacter.player.player_id = playerId;
    unlockedCharacter.character.character_id = characterId;
    unlockedCharacter.level = 1;

    await unlockCharRepo.save(unlockedCharacter);
}

export async function levelUpCharacter(playerId: string, characterId: number) {
    const unlockedCharacter = await unlockCharRepo.findOne({
        where: {
            player: { player_id: playerId },
            character: { character_id: characterId }
        }
    });

    if (unlockedCharacter) {
        unlockedCharacter.level += 1;
        await unlockCharRepo.save(unlockedCharacter);
    } else {
        throw new InternalServerError(`Character with ID ${characterId} not found for player ${playerId}`);
    }
}
