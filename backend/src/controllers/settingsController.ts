import { Request, Response } from 'express';
import { getSettingsByPlayerId, insertSettings } from '../repositories/settingsRepository';
import { BadRequestError, InternalServerError } from '../errors';
import  logger  from '../logger/logger'


export async function getSettings(req: Request, res: Response) {
  const { player_id } = req.body;

  if (!player_id) {
    throw new BadRequestError('Missing player_id');
  }

  try {
    const settings = await getSettingsByPlayerId(player_id);
    if (!settings) {
      res.status(404).json({ message: 'referring user not found' });
      return;
    }
    res.status(200).json(settings);
  } catch (err) {
    logger.error("Failed to retrive settings", err)
    throw new InternalServerError('Failed to retrieve settings');
  }
}


export async function setSettings(req: Request, res: Response) {
  const { player_id, musicVolume, soundVolume } = req.body;

  if (!player_id || musicVolume == null || soundVolume == null) {
    throw new BadRequestError('Missing required fields');
  }

  try {
    const savedSettings = await insertSettings(player_id, musicVolume, soundVolume);
    res.status(200).json(savedSettings);
  } catch (err) {
    logger.error("Failed to save settings", err)
    throw new InternalServerError('Failed to save settings');
  }
}
