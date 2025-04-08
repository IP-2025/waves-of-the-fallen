import { PrismaClient } from '@prisma/client';

let prismaClient: PrismaClient | null = null;

function getPrismaClient(): PrismaClient {
  if (!prismaClient) {
    prismaClient = new PrismaClient();
  }
  return prismaClient;
}

function setPrismaClient(prisma: PrismaClient) {
  prismaClient = prisma;
}

export { getPrismaClient, setPrismaClient };
