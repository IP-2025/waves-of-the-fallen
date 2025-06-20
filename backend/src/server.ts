import 'reflect-metadata';
import app from './app';
import { termLogger as logger } from 'logger';
import { innitAllCharacters } from 'services/characterService';
import { AppDataSource } from 'database/dataSource';
import { AppConfig } from 'core/config';

const port = AppConfig.PORT || 3000;
app.listen(port, () => {
  logger.info(`🚀 Server crying on port ${port}`);
});
AppDataSource.initialize()
  .then(async () => {
    await innitAllCharacters();

  })
  .catch((error) => {
    logger.error(error);
  });
