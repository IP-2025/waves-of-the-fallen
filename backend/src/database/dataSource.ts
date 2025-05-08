import { DataSource } from 'typeorm';
import { DbConfig } from '../core/config';
import { Player } from './entities/Player';
import { Credential } from './entities/Credential';
import { Settings } from './entities/Settings';
import { Character } from './entities/Character';
import { UnlockedCharacter } from './entities/UnlockedCharacter';

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
