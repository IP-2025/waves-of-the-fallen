import { UnauthorizedError } from '../errors';
import { NextFunction, Request, Response } from 'express';
import { extractAndValidatePlayerId } from '../auth/jwt';
import { userExists } from '../repositories/playerRepository';

export async function authenticationStep(req: Request, res: Response, next: NextFunction) {
  try {
    // get token and validate
    const authHeader = req.headers['authorization'];
    console.log('authHeader', authHeader);
    const playerId = extractAndValidatePlayerId(authHeader);

    const result = await userExists(playerId);
    if (!result) {
      return next(new UnauthorizedError('Unauthorized'));
    }

    next();
  } catch (error) {
    next(error)
  }
}
