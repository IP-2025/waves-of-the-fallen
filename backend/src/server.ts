import 'reflect-metadata';
import app from './app';
import { PORT } from './core/config';
import { AppDataSource } from './libs/data-source';
import logger from './logger/logger';

const port = PORT || 3000;

AppDataSource.initialize()
  .then(() => {
    app.listen(port, () => {
      logger.info(`🚀 Server screaming on port ${port}`);
    });
  })
  .catch(error => {
    logger.error(error);
  })
