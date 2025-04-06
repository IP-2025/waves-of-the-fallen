import { prismaClient } from '../libs';

export async function checkDatabaseHealth(): Promise<boolean> {
  try {
    // Perform a simple query to check the database connection
    await prismaClient.$queryRaw`SELECT 1`;
    return true;
  } catch (error) {
    console.error('Database health check failed:', error);
    return false;
  }
}
