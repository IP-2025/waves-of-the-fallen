import 'reflect-metadata';
import app from './app';
import { termLogger as logger } from './logger';
import { innitAllCharacters } from './services/characterService';
import { AppDataSource } from './database/dataSource';
import { AppConfig } from './core/config';

const port = AppConfig.PORT || 3000;

AppDataSource.initialize()
  .then(async () => {
    await innitAllCharacters()
    app.listen(port, () => {
      logger.info(`🚀 Server crying on port ${port}`);
    });
  })
  .catch((error) => {
    logger.error(error);
  });
