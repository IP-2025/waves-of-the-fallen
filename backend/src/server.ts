import 'reflect-metadata';
import app from './app';
import { PORT } from './core/config';
import { AppDataSource } from './libs/data-source';
import logger from './logger/logger';
import { innitAllCharacters } from './services/characterService';

const port = PORT || 3000;

AppDataSource.initialize()
  .then(async () => {
    await innitAllCharacters()
    app.listen(port, () => {
      logger.info(`🚀 Server screaming on port ${port}`);
    });
  })
  .catch((error) => {
    logger.error(error);
  });
