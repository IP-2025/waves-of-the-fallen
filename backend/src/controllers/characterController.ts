import { NextFunction, Request, Response } from 'express';
import { getAllCharacters, getAllUnlockedCharacters, levelUpChar, saveCharChanges } from '../services/characterService';
import {extractAndValidatePlayerId} from "../auth/jwt";
import {BadRequestError} from "../errors";
import logger from "../logger/logger";

export const getAllCharacterController = async (req: Request, res: Response, next: NextFunction) => {
  logger.info("GET: /character")
  try {
    const characters = await getAllCharacters();
    res.json(characters).status(200);
  } catch (err) {
    next(err)
  }
};
export const getAllUnlockedCharacterController = async (req: Request, res: Response, next: NextFunction) => {
  logger.info("POST: /getAllUnlockedCharacters")
  try {
    const playerId = req.body.player_id;
    const characters = await getAllUnlockedCharacters(playerId);
    res.json(characters).status(200);
  } catch (err) {
    next(err)
  }
};

export const unlockCharController = async (req: Request, res: Response, next: NextFunction) => {
  logger.info("POST: /character/unlock")
  try {
    const playerId = extractAndValidatePlayerId(req.headers['authorization'])
    const { character_id: charId } = req.body as { character_id: number };

    if (!playerId || !charId) {
      throw new BadRequestError('Missing required fields');
    }

    await saveCharChanges(playerId, charId);

    res.status(200).json({ message: 'Character unlocked successfully'});

  } catch (err) {
    next(err)
  }
}
export const levelUpCharController = async (req: Request, res: Response, next: NextFunction) => {
  logger.info("POST: /character/levelUpCharController")
  try {
    const playerId = extractAndValidatePlayerId(req.headers['authorization'])
    const { character_id: charId } = req.body as { character_id: number };

    if (!playerId || !charId) {
      throw new BadRequestError('Missing required fields');
    }

    await levelUpChar(playerId, charId);

    res.status(200).json({ message: 'Character unlocked successfully'});

  } catch (err) {
    next(err)
  }
}