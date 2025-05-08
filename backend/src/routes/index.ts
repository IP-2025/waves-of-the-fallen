import { Application } from 'express';
import healthRouter from './health';
import authRouter from './auth';
import protectedRouter from './protected';
import { PREFIX_ROUTE } from '../core/url';

export default function registerRoutes(app: Application): void {
  app.use(`${PREFIX_ROUTE}/protected`, protectedRouter);
  app.use(`${PREFIX_ROUTE}/healthz`, healthRouter);
  app.use(`${PREFIX_ROUTE}/auth`, authRouter);
}
