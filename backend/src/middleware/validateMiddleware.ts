import { InternalServerError, UnauthorizedError } from '../errors';
import { NextFunction, Request, Response } from 'express';
import { verifyToken } from '../auth/jwt';
import { userExists } from '../repositories/playerRepository';

export async function authenticationStep(req: Request, res: Response, next: NextFunction) {
  try {
    // get token and validate
    const authHeader = req.headers['authorization'];
    if (!authHeader) {
      return next(new UnauthorizedError('Unauthorized'));
    }

    const token = authHeader.split(' ')[1];
    if (!token) {
      return next(new UnauthorizedError('Unauthorized'));
    }

    const playerId = verifyToken(token);
    if (!playerId) {
      return next(new UnauthorizedError('Unauthorized'));
    }

    const result = await userExists(playerId);
    if (!result) {
      return next(new UnauthorizedError('Unauthorized'));
    }

    next();
  } catch (error) {
    next(new InternalServerError('Internal Server Error'));
  }
}
