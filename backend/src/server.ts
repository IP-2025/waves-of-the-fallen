import 'reflect-metadata';
import app from './app';
import logger from './logger/termLogger';
import { innitAllCharacters } from './services/characterService';
import { AppDataSource } from './database/data-source';
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
