import { NextFunction, Request, Response } from 'express';
import { BadRequestError } from '../errors';
import { registerUser } from '../services/registerService';

export async function registrateController(req: Request, res: Response, next: NextFunction) {
  try {
    const { username, password, mail } = req.body;

    if (!username || !password || !mail) {
      throw new BadRequestError('Mail, Username or Password is required');
    }

    await registerUser(username, password, mail);
    res.status(201).json({ message: `User ${username} registered` });
  } catch (error) {
    next(error);
  }
}