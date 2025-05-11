import { NextFunction, Request, Response } from 'express';
import { getAllCharacters, getAllUnlockedCharacters } from 'services/characterService';
import { BadRequestError } from '../errors';

export async function getAllCharacterController(req: Request, res: Response, next: NextFunction) {
  try {
    const characters = await getAllCharacters();
    res.status(200).json(characters);
  } catch (err) {
    next(err);
  }
}

export async function getAllUnlockedCharacterController(req: Request, res: Response, next: NextFunction) {
  try {
    const playerId = req.body?.player_id;
    if (!playerId) {
      throw new BadRequestError('Player ID is required');
    }

    const characters = await getAllUnlockedCharacters(playerId);
    res.status(200).json(characters);
  } catch (err) {
    next(err);
  }
}