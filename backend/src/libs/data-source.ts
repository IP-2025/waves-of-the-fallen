import { DataSource } from 'typeorm';
import { Player } from './entities/Player';
import { Credential } from './entities/Credential';

export const AppDataSource = new DataSource({
  type: 'postgres',
  host: process.env.DB_HOST || 'localhost',
  port: parseInt(process.env.DB_PORT || '5432', 10),
  username: process.env.DB_USER || 'postgres',
  password: process.env.DB_PASSWORD || 'postgres',
  database: process.env.DB_DATABASE || 'backend_db',
  synchronize: true,
  logging: false,
  entities: [Player, Credential], // Include your entities here
  migrations: [],
});
