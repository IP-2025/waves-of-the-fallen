import { Request, Response } from 'express';
import { checkDatabaseHealth } from '../services/healthService';

export async function healthCheck(_req: Request, res: Response) {
  const isDbHealthy = await checkDatabaseHealth();

  if (isDbHealthy) {
    res.status(200).json({ status: 'ok' });
  } else {
    res.status(500).json({ status: 'error', message: 'Database is not healthy' });
  }
}
