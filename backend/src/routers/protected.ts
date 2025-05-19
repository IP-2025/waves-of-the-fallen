import express from 'express';
import { authenticationStep } from 'middleware';
import {
  getGoldController,
  getSettings,
  setGoldController,
  setSettings,
  getAllCharacterController,
  getAllUnlockedCharacterController,
  levelUpCharController,
  progressSyncController,
  unlockCharController,
  updateUserHighscore, getUserHighscore, getTopHighscore
} from 'controllers';

const protectedRouter = express.Router();

protectedRouter.get('/', authenticationStep, (req, res) => {
  res.json({ authenticated: true });
});
protectedRouter.post('/getSettings', authenticationStep, getSettings);
protectedRouter.post('/setSettings', authenticationStep, setSettings);
protectedRouter.post('/setGold', authenticationStep, setGoldController);
protectedRouter.post('/getGold', authenticationStep, getGoldController);
protectedRouter.get('/characters', authenticationStep, getAllCharacterController);
protectedRouter.post('/getAllUnlockedCharacters', authenticationStep, getAllUnlockedCharacterController);
protectedRouter.post('/character/unlock', authenticationStep, unlockCharController);
protectedRouter.post('/character/levelUp', authenticationStep, levelUpCharController);
protectedRouter.post('/progressSync', authenticationStep, progressSyncController);

protectedRouter.post('highscore/update', authenticationStep, updateUserHighscore)
protectedRouter.post('highscore/getUserHighscore', authenticationStep, getUserHighscore)
protectedRouter.get('highscore/top', authenticationStep, getTopHighscore)
export default protectedRouter;
