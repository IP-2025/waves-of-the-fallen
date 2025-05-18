import express from 'express';
import { authenticationStep } from 'middleware';
import { getGoldController, getSettings, setGoldController, setSettings, getAllCharacterController, getAllUnlockedCharacterController, levelUpCharController, progressSyncController, unlockCharController} from 'controllers';

const protectedRouter = express.Router();

protectedRouter.get('/', authenticationStep, (req, res) => {
  res.json({ authenticated: true });
});
protectedRouter.post('/getSettings', authenticationStep, getSettings);
protectedRouter.post('/setSettings', authenticationStep, setSettings);
protectedRouter.post('/getAllCharacters', authenticationStep, getAllCharacterController);
protectedRouter.post('/setGold', authenticationStep, setGoldController);
protectedRouter.post('/getGold', authenticationStep, getGoldController);
// TODO Why two time triggering same Controlller Get and Post
protectedRouter.get('/character', authenticationStep, getAllCharacterController);
protectedRouter.post('/getAllUnlockedCharacters', authenticationStep, getAllUnlockedCharacterController);
protectedRouter.post('/character/unlock', authenticationStep, unlockCharController);
protectedRouter.post('/character/levelUp', authenticationStep, levelUpCharController);
protectedRouter.post('/progressSync', authenticationStep, progressSyncController);

export default protectedRouter;
