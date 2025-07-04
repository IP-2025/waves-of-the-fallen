import { Request, Response, NextFunction } from 'express';
import { checkDatabaseHealth } from 'services';
import { termLogger as logger } from 'logger';
import { InternalServerError } from 'errors';

export async function healthCheck(_req: Request, res: Response, next: NextFunction) {
  try {
    // const isDbHealthy = await checkDatabaseHealth();

    if (true) {
      res.status(200).json({ status: 'ok' });
    } else {
      logger.error('Database is not healthy');
      throw new InternalServerError('Database is not healthy');
    }
  } catch (error) {
    next(error);
  }
}
