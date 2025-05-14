import express from 'express';
import { authenticationStep } from '../middleware/validateMiddleware';
import { getSettings, setSettings } from '../controllers/settingsController';
import { setGoldController, getGoldController } from '../controllers/playerStatController';
import {
  getAllCharacterController,
  getAllUnlockedCharacterController, levelUpCharController, progressSyncController,
  unlockCharController,
} from '../controllers/characterController';

const protectedRouter = express.Router();

protectedRouter.get('/', authenticationStep, (req, res) => {
  res.json({ authenticated: true });
});
protectedRouter.post('/getSettings', authenticationStep, getSettings);
protectedRouter.post('/setSettings', authenticationStep, setSettings);
protectedRouter.post('/getAllCharacters',authenticationStep,getAllCharacterController);
protectedRouter.post('/setGold',authenticationStep,setGoldController);
protectedRouter.post('/getGold',authenticationStep,getGoldController);
protectedRouter.get('/character',authenticationStep, getAllCharacterController);
protectedRouter.post('/getAllUnlockedCharacters',authenticationStep, getAllUnlockedCharacterController);
protectedRouter.post('/character/unlock', authenticationStep, unlockCharController)
protectedRouter.post('/character/levelUp', authenticationStep, levelUpCharController)
protectedRouter.post('/progressSync', authenticationStep, progressSyncController);


export default protectedRouter;
