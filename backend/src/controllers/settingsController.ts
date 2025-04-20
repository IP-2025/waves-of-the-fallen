import { Request, Response } from 'express';
import { getSettingsByPlayerId, upsertSettings } from '../repositories/settingsRepository';
import { BadRequestError, InternalServerError } from '../errors';


export async function fetchSettings(req: Request, res: Response) {
  const { player_id } = req.body;

  if (!player_id) {
    throw new BadRequestError('Missing player_id');
  }

  try {
    const settings = await getSettingsByPlayerId(player_id);
    if (!settings) {
      return res.status(404).json({ message: 'Settings not found' });
    }
    res.json(settings);
  } catch (err) {
    console.error(err);
    throw new InternalServerError('Failed to retrieve settings');
  }
}


export async function setSettings(req: Request, res: Response) {
  const { player_id, musicVolume, soundVolume } = req.body;

  if (!player_id || musicVolume == null || soundVolume == null) {
    throw new BadRequestError('Missing required fields');
  }

  try {
    const savedSettings = await upsertSettings(player_id, musicVolume, soundVolume);
    res.status(200).json(savedSettings);
  } catch (err) {
    console.error(err);
    throw new InternalServerError('Failed to save settings');
  }
}
