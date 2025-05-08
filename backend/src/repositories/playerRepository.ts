import {AppDataSource} from '../database/dataSource';
import {Player} from '../database/entities/Player';
import { termLogger as logger } from '../logger';
import {ConflictError, InternalServerError} from '../errors';
import {Settings} from '../database/entities/Settings';

const playersRepo = AppDataSource.getRepository(Player);
const settingRepo = AppDataSource.getRepository(Settings);

export interface NewPlayer {
    player_id: string;
    username: string;
}

export async function createNewPlayer(user: NewPlayer) {
    const existingPlayer = await playersRepo.findOneBy({ username: user.username });
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
    try {
        const player = await playersRepo.findOneBy({player_id: playerId});
        if (!player) {
            throw new InternalServerError('Player not found');
        }
        return player.gold;
    } catch (error) {
        logger.error('Error retrieving gold: ', error);
        throw new InternalServerError('Error retrieving gold');
    }
}


export async function setGoldRepository(playerId: string, gold: number): Promise<void> {
    try {
        const result = await playersRepo.update({player_id: playerId}, {gold});
        if (result.affected === 0) {
            throw new InternalServerError('Player not found');
        }
        logger.debug(`Gold updated for player_id: ${playerId}, new gold: ${gold}`);
    } catch (error) {
        logger.error('Error setting gold: ', error);
        throw new InternalServerError('Error setting gold');
    }
}

export async function deletePlayer(playerId: string): Promise<void> {
    try {
        await AppDataSource.transaction(async (transactionalEntityManager) => {
            await transactionalEntityManager.delete(Settings, {player_id: playerId});
            await transactionalEntityManager.delete(Player, {player_id: playerId});
        });
        logger.info(`Player with ID ${playerId} deleted successfully.`);
    } catch (error) {
        logger.error('Error deleting player: ', error);
        throw new InternalServerError('Error deleting player');
    }
}
