import app from './app';
import { PORT } from './core/config';
import logger from './logger/logger';

const port = PORT || 3000;

app.listen(port, () => {
  logger.info(`🚀 Server screaming on port ${port}`);
});
