import { NextFunction, Request, RequestHandler, Response } from 'express';
import { getGoldService, setGoldService } from '../services/playerService';
import { BadRequestError } from '../errors';

export const getGoldController: RequestHandler = async (req, res) => {
  try {
    const playerId = req.body.player_id;
    const gold = await getGoldService(playerId);
    res.json(gold).status(200);
  } catch (err) {
    res.status(500).send('Server error');
  }
};

export async function setGoldController(req: Request, res: Response, next: NextFunction) {
  try {
    const { playerid, gold } = req.body;
    if (!playerid || !gold) {
      throw new BadRequestError('Email or Password is required');
    }
    await setGoldService(playerid, gold);
    res.status(200);
  } catch (err) {
    res.status(500).send('Server error');
  }
}
