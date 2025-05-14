import { Application } from 'express';
import { PREFIX_ROUTE } from 'core/url';
import healthRouter from './health';
import authRouter from './auth';
import protectedRouter from './protected';
import gameRouter from './game';

export default function createRoutes(app: Application): void {
  app.use(`${PREFIX_ROUTE}/protected`, protectedRouter);
  app.use(`${PREFIX_ROUTE}/healthz`, healthRouter);
  app.use(`${PREFIX_ROUTE}/auth`, authRouter);
  app.use(`${PREFIX_ROUTE}`, gameRouter)
}
