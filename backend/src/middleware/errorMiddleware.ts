import { Request, Response, NextFunction } from 'express';
import logger from '../logger/termLogger';
import { CustomError } from '../errors/customErorr';

export function errorHandler(err: CustomError, req: Request, res: Response, next: NextFunction) {
  logger.error(`Error: ${err.message}`);

  const status = err.status || 500;
  const message = err.message || 'Internal Server Error';

  res.status(status).json({
    status: 'error',
    message,
  });
}
