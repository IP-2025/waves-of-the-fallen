import { DataSource } from 'typeorm';
import { DbConfig } from '../core/config';
import { Character, Player, Settings, UnlockedCharacter, Credential } from './entities';

export const AppDataSource = new DataSource({
  type: 'postgres',
  host: DbConfig.HOST,
  port: DbConfig.PORT,
  username: DbConfig.USERNAME,
  password: DbConfig.PASSWORD,
  database: DbConfig.DATABASE,
  synchronize: true,
  logging: false,
  entities: [Player, Credential, Settings, Character, UnlockedCharacter],
  migrations: [],
});
