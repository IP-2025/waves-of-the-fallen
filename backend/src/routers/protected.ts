import express from 'express';
import { authenticationStep } from 'middleware';
import {
  getGoldController,
  getSettings,
  setGoldController,
  setSettings,
  getAllCharacterController,
  getProgress,
  levelUpCharController,
  progressSyncController,
  unlockCharController,
  updateUserHighscore, getUserHighscore, getTopHighscore, addGoldController
} from 'controllers';

const protectedRouter = express.Router();

protectedRouter.get('/', authenticationStep, (req, res) => {
  res.json({ authenticated: true });
});
protectedRouter.post('/getSettings', authenticationStep, getSettings);
protectedRouter.post('/setSettings', authenticationStep, setSettings);

protectedRouter.post('/setGold', authenticationStep, setGoldController);
protectedRouter.post('/addGold', authenticationStep, addGoldController);
protectedRouter.post('/getGold', authenticationStep, getGoldController);

protectedRouter.get('/characters', authenticationStep, getAllCharacterController);
protectedRouter.post('/character/unlock', authenticationStep, unlockCharController);
protectedRouter.post('/character/levelUp', authenticationStep, levelUpCharController);

protectedRouter.post('/progress', authenticationStep, getProgress);
protectedRouter.post('/progress/sync', authenticationStep, progressSyncController);

protectedRouter.post('/highscore/update', authenticationStep, updateUserHighscore)
protectedRouter.post('/highscore/getUserHighscore', authenticationStep, getUserHighscore)
protectedRouter.get('/highscore/top', authenticationStep, getTopHighscore)

export default protectedRouter;
