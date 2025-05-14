import {NextFunction, Request, Response} from 'express';
import {getSettingsByPlayerId, insertSettings} from 'repositories/settingsRepository';
import {BadRequestError, UnauthorizedError} from 'errors';
import {termLogger as logger} from 'logger';
import {extractAndValidatePlayerId} from 'auth/jwt';

export async function getSettings(req: Request, res: Response, next: NextFunction) {
    try {
        const player_id = extractAndValidatePlayerId(req.headers['authorization']);

        if (!player_id) {
            throw new UnauthorizedError('Player ID not found');
        }

        const settings = await getSettingsByPlayerId(player_id);
        if (!settings) {
            throw new BadRequestError('Settings not found');
        }
        res.status(200).json(settings);
    } catch (err) {
        next(err)
    }
}

export async function setSettings(req: Request, res: Response, next: NextFunction) {
    try {
        const player_id = extractAndValidatePlayerId(req.headers['authorization']);
        if (!player_id) {
            throw new UnauthorizedError('Player ID not found');
        }

        const {musicVolume, soundVolume} = req.body;
        if (musicVolume == null || soundVolume == null) {
            throw new BadRequestError('Missing required fields');
        }

        if (musicVolume > 100 || soundVolume > 100) {
            throw new BadRequestError('Wrong values');
        }

        if (musicVolume < 0 || soundVolume < 0) {
            throw new BadRequestError('Wrong values');
        }

        const savedSettings = await insertSettings(player_id, musicVolume, soundVolume);
        res.status(200).json(savedSettings);
    } catch (err) {
        logger.error('Failed to save settings', err);
        next(err)
    }
}
