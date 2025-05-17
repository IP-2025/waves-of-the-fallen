import { NextFunction, Request, Response } from 'express';
import { BadRequestError } from '../errors';
import { pwdCheck } from '../services/loginService';

export async function loginController(req: Request, res: Response, next: NextFunction) {
  try {
    const { email, password } = req.body;
    if (!email || !password) {
      throw new BadRequestError('Email or Password is required');
    }
    const token = await pwdCheck(email, password);

    res.cookie('token', token, {
      httpOnly: true,
      secure: true,
      sameSite: 'strict',
    });
    res.status(200).json({ message: `Login successful for ${email}`, token: token });
  } catch (error) {
    next(error);
  }
}
