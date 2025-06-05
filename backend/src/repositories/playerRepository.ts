import {AppDataSource} from 'database/dataSource';
import {termLogger as logger} from 'logger';
import { ConflictError, InternalServerError, NotFoundError} from 'errors';
import {HighScore, Player, Settings, UnlockedCharacter} from 'database/entities';

const playersRepo = AppDataSource.getRepository(Player);
const settingRepo = AppDataSource.getRepository(Settings);

export interface NewPlayer {
    player_id: string;
    username: string;
}

export async function createNewPlayer(user: NewPlayer) {
    const existingPlayer = await playersRepo.findOneBy({username: user.username});
    if (existingPlayer) {
        throw new ConflictError('Player with this username already exists');
    }

    const player = new Player();
    player.player_id = user.player_id;
    player.username = user.username;

    const setting = new Settings();
    setting.player_id = user.player_id;
    setting.musicVolume = 0;
    setting.soundVolume = 0;

    await settingRepo.save(setting);
    return await playersRepo.save(player);
}

export async function userExists(playerId: string): Promise<boolean> {
    try {
        const player = await playersRepo.findOneBy({player_id: playerId});
        return player !== null;
    } catch (error) {
        logger.error('Error checking if user exists: ', error);
        throw new InternalServerError('Error checking if user exists');
    }
}

export async function getGoldRepository(playerId: string): Promise<number> {
    const player = await playersRepo.findOneBy({player_id: playerId});
    if (!player) {
        throw new NotFoundError('Player not found');
    }
    return player.gold;
}

export async function setGoldRepository(playerId: string, gold: number): Promise<void> {
    const result = await playersRepo.update({player_id: playerId}, {gold});
    if (result.affected === 0) {
        throw new NotFoundError('Player not found');
    }
}

export async function deletePlayer(playerId: string): Promise<void> {
    try {
        await AppDataSource.transaction(async (transactionalEntityManager) => {
            await transactionalEntityManager.delete(HighScore, { player: { player_id: playerId } });
            await transactionalEntityManager.delete(UnlockedCharacter, { player: { player_id: playerId } });
            await transactionalEntityManager.delete(Credential, { player: { player_id: playerId } });
            await transactionalEntityManager.delete(Settings, { player_id: playerId });
            await transactionalEntityManager.delete(Player, { player_id: playerId });
        });
        logger.info(`Player with ID ${playerId} und alle zugehörigen Daten gelöscht.`);
    } catch (error) {
        logger.error('Error deleting player: ', error);
        throw new InternalServerError('Error deleting player');
    }
}