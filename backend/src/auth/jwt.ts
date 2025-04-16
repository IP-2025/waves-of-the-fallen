import jwt, { JwtPayload } from 'jsonwebtoken';
import { JWT_SECRET } from '../core/config';
import { InternalServerError } from '../errors';

export function generateToken(userId: string): string {
  if (!JWT_SECRET) {
    throw new InternalServerError('JWT_SECRET is not defined');
  }
  const token = jwt.sign({ id: userId }, JWT_SECRET, { expiresIn: '7d' });
  return token;
}

export function verifyToken(token: string): string | null {
  if (!JWT_SECRET) {
    throw new InternalServerError('JWT_SECRET is not defined');
  }
  try {
    const decoded = jwt.verify(token, JWT_SECRET) as JwtPayload;
    return decoded.id as string;
  } catch (error) {
    return null;
  }
}
