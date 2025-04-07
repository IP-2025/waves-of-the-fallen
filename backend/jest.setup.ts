import { PostgreSqlContainer, StartedPostgreSqlContainer } from "@testcontainers/postgresql";
import { PrismaClient } from '@prisma/client';
import { execSync } from 'child_process';
import { setPrismaClient } from "./src/libs/prismaClient";

let prisma: PrismaClient;
let container: StartedPostgreSqlContainer;

global.beforeAll(async () => {
  container = await new PostgreSqlContainer().start();
  prisma = new PrismaClient();

  const urlConnection = container.getConnectionUri();
  process.env.DATABASE_URL = urlConnection;

  setPrismaClient(prisma);

  execSync(`prisma migrate deploy`, {
    env: {
      ...process.env,
      DATABASE_URL: urlConnection,
    },
  });
});

global.afterAll(async () => {
  await prisma.$disconnect();
  await container.stop();
});

// clears testcontainer after each test
global.afterEach(async () => {
  // Reset DB by dropping all tables
  await prisma.$executeRawUnsafe(`DROP SCHEMA public CASCADE;`);
  await prisma.$executeRawUnsafe(`CREATE SCHEMA public;`);

  // Re-apply schema
  execSync(`prisma migrate deploy`, {
    env: {
      ...process.env,
      DATABASE_URL: container.getConnectionUri(),
    },
  });
});

describe('print out tables', () => {
  it('check if prisma worked', async () => {
    const tables = await prisma.$queryRaw`SELECT table_name FROM information_schema.tables WHERE table_schema='public'`;
    console.log(tables);
  });
});

export { prisma };
