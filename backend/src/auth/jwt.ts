import jwt, { JwtPayload } from 'jsonwebtoken';
import { JWT_SECRET } from '../core/config';
import { InternalServerError, UnauthorizedError } from '../errors';

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

export function extractAndValidatePlayerId(authHeader?: string): string {
  if (!authHeader) {
    throw new UnauthorizedError('Unauthorized');
  }

  const token = authHeader.split(' ')[1];
  if (!token) {
    throw new UnauthorizedError('Unauthorized');
  }

  const playerId = verifyToken(token);
  if (!playerId) {
    throw new UnauthorizedError('Unauthorized');
  }
  return playerId;
}
