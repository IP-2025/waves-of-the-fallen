import { PostgreSqlContainer, StartedPostgreSqlContainer } from "@testcontainers/postgresql";
import { PrismaClient } from '@prisma/client';
import { execSync } from 'child_process';
import { checkDatabaseHealth } from '../src/services/healthService';
import { setPrismaClient } from "../src/libs/prismaClient";

describe('test healthService', () => {
  let prisma: PrismaClient;
  let container: StartedPostgreSqlContainer;

  beforeAll(async () => {
    container = await new PostgreSqlContainer().start();
    prisma = new PrismaClient

    const urlConnection = container.getConnectionUri()
    process.env.DATABASE_URL = urlConnection;
    console.log(urlConnection)

    setPrismaClient(prisma)

    execSync(`prisma migrate deploy`, {
      env: {
        ...process.env,
        DATABASE_URL: urlConnection,
      },
    });
  });

  afterAll(async () => {
    await container.stop();
  });

  it('check for migration', async () => {
    const tables = await prisma.$queryRaw`SELECT table_name FROM information_schema.tables WHERE table_schema='public'`;
    console.log(tables);
  })

  it('test databse health function', async () => {
    const reslut = await checkDatabaseHealth()
    expect(reslut).toBe(true)
  })
})
