import { getPrismaClient } from '../libs/prismaClient';

export async function checkDatabaseHealth(): Promise<boolean> {
  try {
    const prisma = getPrismaClient();
    // Perform a simple query to check the database connection
    await prisma.$queryRaw`SELECT 1`;
    return true;
  } catch (error) {
    console.error('Database health check failed:', error);
    return false;
  }
}
