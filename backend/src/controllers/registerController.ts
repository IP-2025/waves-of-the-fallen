import { NextFunction, Request, Response } from 'express';
import { BadRequestError } from 'errors';
import { registerUser } from 'services';

export async function registrateController(req: Request, res: Response, next: NextFunction) {
  try {
    const { username, password, email } = req.body;

    if (!username || !password || !email) {
      throw new BadRequestError('Mail, Username or Password is required');
    }

    const playerId = await registerUser(username, password, email);

    res.status(201).json({
      message: `User ${username} registered`,
      player_id: playerId,
    });
  } catch (error) {
    next(error);
  }
}
