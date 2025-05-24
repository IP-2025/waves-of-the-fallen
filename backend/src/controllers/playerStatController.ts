import { NextFunction, Request, Response } from 'express';
import {addGoldService, getGoldService, setGoldService} from 'services';
import { BadRequestError } from 'errors';
import {extractAndValidatePlayerId} from "auth/jwt";

export async function getGoldController(req: Request, res: Response, next: NextFunction) {
  try {
    const playerId = req.body.player_id;
    const gold = await getGoldService(playerId);
    res.json(gold).status(200);
  } catch (err) {
    next(err);
  }
}

export async function setGoldController(req: Request, res: Response, next: NextFunction) {
  try {
    const { player_id, gold } = req.body;
    if (!player_id || !gold) {
      throw new BadRequestError('Email or Password is required');
    }
    await setGoldService(player_id, gold);
    res.status(200).send('OK');
  } catch (err) {
    next(err);
  }
}

export async function addGoldController(req: Request, res: Response, next: NextFunction) {
    try {
      const playerId = extractAndValidatePlayerId(req.headers['authorization']);
      const { gold } = req.body.gold;
        if (!playerId || !gold) {
        throw new BadRequestError('Player ID and Gold amount are required');
        }
        await addGoldService(playerId, gold);
        res.status(200).send('OK');
    } catch (err) {
        next(err);
    }
}
