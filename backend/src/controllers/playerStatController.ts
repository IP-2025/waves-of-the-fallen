import { NextFunction, Request, RequestHandler, Response } from 'express';
import { getGoldService, setGoldService } from 'services';
import { BadRequestError } from 'errors';

export const getGoldController: RequestHandler = async (req, res, next) => {
  try {
    const playerId = req.body.player_id;
    const gold = await getGoldService(playerId);
    res.json(gold).status(200);
  } catch (err) {
    next(err);
  }
};

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
