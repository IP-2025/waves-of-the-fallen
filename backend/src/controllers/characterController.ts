import {NextFunction, Request, Response} from 'express';
import {
    getAllCharacters,
    getAllUnlockedCharacters,
    levelUpChar,
    progressSyncService,
    saveCharChanges,
} from 'services/characterService';
import {extractAndValidatePlayerId} from '../auth/jwt';
import {BadRequestError} from '../errors';
import {termLogger as logger} from 'logger';
import {getGoldService} from "../services";

export async function getAllCharacterController(req: Request, res: Response, next: NextFunction) {
    try {
        const characters = await getAllCharacters();
        res.status(200).json(characters);
    } catch (err) {
        next(err);
    }
};
export const getProgress = async (req: Request, res: Response, next: NextFunction) => {
    try {
        const playerId = extractAndValidatePlayerId(req.headers['authorization']);
        if (!playerId) {
            throw new BadRequestError('Missing required fields');
        }
        const gold = await getGoldService(playerId);
        const characters = await getAllUnlockedCharacters(playerId);
        res.status(200).json({
            gold: gold,
            unlocked_characters: characters
        });
    } catch (err) {
        next(err);
    }
};

export const unlockCharController = async (req: Request, res: Response, next: NextFunction) => {
    logger.info('POST: /character/unlock');
    try {
        const playerId = extractAndValidatePlayerId(req.headers['authorization']);
        if (!playerId) {
            throw new BadRequestError('Missing required fields');
        }
        const {character_id: charId} = req.body as { character_id: number };

        if (!playerId || !charId) {
            throw new BadRequestError('Missing required fields');
        }

        await saveCharChanges(playerId, charId);

        res.status(200).json({message: 'Character unlocked successfully'});

    } catch (err) {
        next(err);
    }
};
export const levelUpCharController = async (req: Request, res: Response, next: NextFunction) => {
    logger.info('POST: /character/levelUpCharController');
    try {
        const playerId = extractAndValidatePlayerId(req.headers['authorization']);
        const {character_id: charId} = req.body as { character_id: number };

        if (!playerId || !charId) {
            throw new BadRequestError('Missing required fields');
        }

        await levelUpChar(playerId, charId);

        res.status(200).json({message: 'Character unlocked successfully'});

    } catch (err) {
        next(err);
    }
};

export const progressSyncController = async (req: Request, res: Response, next: NextFunction) => {
    try {
        const playerId = extractAndValidatePlayerId(req.headers['authorization']);
        if (!playerId) {
            throw new BadRequestError('Missing required fields');
        }

        await progressSyncService(playerId, req.body)

        res.status(200).json({message: 'Character unlocked successfully'});

    } catch (err) {
        next(err);
    }
}