import express from 'express';
import characterRouter from './character';
import progressRouter from './progress';
import highscoreRouter from './highscore';
import settingsRouter from './settings';
import goldRouter from './gold';
import { authenticationStep } from 'middleware';

const protectedRouter = express.Router();

protectedRouter.get('/', authenticationStep, (req, res) => {
  res.json({ authenticated: true });
});

protectedRouter.use('/character', characterRouter);
protectedRouter.use('/progress', progressRouter);
protectedRouter.use('/highscore', highscoreRouter);
protectedRouter.use('/settings', settingsRouter);
protectedRouter.use('/gold', goldRouter);

export default protectedRouter;