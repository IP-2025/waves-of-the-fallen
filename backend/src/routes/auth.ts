import { Router, Request, Response, NextFunction } from 'express';
import { BadRequestError, UnauthorizedError } from '../errors';

const router = Router();

router.post('/login', (req: Request, res: Response, next: NextFunction) => {
  try {
    const { username, password } = req.body;
    if (!username || !password) {
      throw new BadRequestError('Username or Password is required');
    }
    // TODO: validate and authenticate user
    res.status(200).json({ message: `Login successful for ${username}` });
  } catch (error) {
    next(error);
  }
});

router.post('/register', (req: Request, res: Response, next: NextFunction) => {
  try {
    const { username, password } = req.body;
    if (!username || !password) {
      throw new BadRequestError('Username or Password is required');
    }

    res.status(201).json({ message: `User ${username} registered` });
  } catch (error) {
    next(error);
  }
});

export default router;
