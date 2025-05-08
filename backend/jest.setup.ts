import { PostgreSqlContainer, StartedPostgreSqlContainer } from "@testcontainers/postgresql";
import { AppDataSource } from './src/database/dataSource';

let container: StartedPostgreSqlContainer;

jest.setTimeout(30000); // Set timeout to 30 seconds

global.beforeAll(async () => {
  container = await new PostgreSqlContainer().start();

  const urlConnection = container.getConnectionUri();
  process.env.DATABASE_URL = urlConnection;

  // Update TypeORM data source options dynamically
  AppDataSource.setOptions({
    url: urlConnection,
  });

  await AppDataSource.initialize();
});

global.afterAll(async () => {
  await AppDataSource.destroy();
  await container.stop();
});

// clears testcontainer after each test
global.afterEach(async () => {
  // Reset DB by dropping all tables
  await AppDataSource.query(`DROP SCHEMA public CASCADE; CREATE SCHEMA public;`);

  // Re-apply schema
  await AppDataSource.synchronize(true);
});
