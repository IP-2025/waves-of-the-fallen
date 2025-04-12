import { Request, Response } from 'express';
import { checkDatabaseHealth } from '../services/healthService';
import logger from '../logger/logger';

export async function healthCheck(_req: Request, res: Response) {
  const isDbHealthy = await checkDatabaseHealth();

  if (isDbHealthy) {
    res.status(200).json({ status: 'ok' });
  } else {
    logger.error('Database is not healthy');
    res
      .status(500)
      .json({ status: 'error', message: 'Database is not healthy' });
  }
}
