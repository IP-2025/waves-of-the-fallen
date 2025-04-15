import { NextFunction, Request, Response } from 'express';
import { BadRequestError, InternalServerError } from '../errors';
import { registerUser } from '../services/registerService';

export async function registrateController(req: Request, res: Response, next: NextFunction) {
  try {
    const { username, password, email: email } = req.body;

    if (!username || !password || !email) {
      throw new BadRequestError('Mail, Username or Password is required');
    }

    await registerUser(username, password, email);
    res.status(201).json({ message: `User ${username} registered` });
  } catch (error) {
    if (error instanceof Error) {
      next(new InternalServerError('Failed to register user. Please try again later.'));
    } else {
      next(error);
    }
  }
}
