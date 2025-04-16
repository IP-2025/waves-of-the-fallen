import { AppDataSource } from '../libs/data-source';
import logger from '../logger/logger';

export async function checkDatabaseHealth(): Promise<boolean> {
  try {
    // Perform a simple query to check the database connection
    await AppDataSource.query('SELECT 1');
    return true;
  } catch (error) {
    logger.error('Database health check failed:', error);
    return false;
  }
}
