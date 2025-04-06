import { PrismaClient } from '@prisma/client';

let prismaClient = new PrismaClient();

function setPrismaClient(prisma: PrismaClient) {
  prismaClient = prisma;
}
export { prismaClient, setPrismaClient };
