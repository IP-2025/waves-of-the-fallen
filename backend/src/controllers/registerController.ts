import { NextFunction, Request, Response } from 'express';
import { BadRequestError, InternalServerError } from '../errors';
import { registerUser } from '../services/registerService';
import logger from "../logger/logger";

export async function registrationController(
  req: Request,
  res: Response,
  next: NextFunction
) {
  logger.info("POST: /register")
  try {
    const { username, password, email } = req.body;

    if (!username || !password || !email) {
      throw new BadRequestError('Mail, Username or Password is required');
    }

    logger.info(`POST: /register - ${username} - ${email}`);
    const playerId = await registerUser(username, password, email);

    res.status(201).json({
      message: `User ${username} registered`,
      player_id: playerId,
    });
  } catch (error) {
    next(error)
  }
}
