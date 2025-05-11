import express from 'express';
import { authenticationStep } from 'middleware';
import { getSettings, setSettings } from 'controllers';
import { getAllCharacterController, getAllUnlockedCharacterController } from 'controllers';
import { setGoldController, getGoldController } from 'controllers';

const protectedRouter = express.Router();

protectedRouter.get('/', authenticationStep, (req, res) => {
  res.json({ authenticated: true });
});
protectedRouter.post('/getSettings', authenticationStep, getSettings);
protectedRouter.post('/setSettings', authenticationStep, setSettings);
protectedRouter.post('/getAllCharacters', authenticationStep, getAllCharacterController);
protectedRouter.post('/getAllUnlockedCharacters', authenticationStep, getAllUnlockedCharacterController);
protectedRouter.post('/setGold', authenticationStep, setGoldController);
protectedRouter.post('/getGold', authenticationStep, getGoldController);

export default protectedRouter;
