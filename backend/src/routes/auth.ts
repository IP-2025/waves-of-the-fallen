import { Router, Request, Response, NextFunction } from 'express';
import { BadRequestError } from '../errors';
import { registrateController } from '../controllers/registerController';

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

router.post('/register', registrateController)

export default router;
