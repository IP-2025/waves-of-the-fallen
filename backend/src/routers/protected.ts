import express from 'express';
import { authenticationStep } from '../middleware/authMiddleware';
import { getSettings, setSettings } from '../controllers/settingsController';
import {
  getAllCharacterController,
  getAllUnlockedCharacterController,
} from '../controllers/characterController';
import { setGoldController, getGoldController } from '../controllers/playerStatController';

const protectedRouter = express.Router();

protectedRouter.get('/', authenticationStep, (req, res) => {
  res.json({ authenticated: true });
});
protectedRouter.post('/getSettings', authenticationStep, getSettings);
protectedRouter.post('/setSettings', authenticationStep, setSettings);
protectedRouter.post('/getAllCharacters', authenticationStep, getAllCharacterController);
protectedRouter.post(
  '/getAllUnlockedCharacters',
  authenticationStep,
  getAllUnlockedCharacterController
);
protectedRouter.post('/setGold', authenticationStep, setGoldController);
protectedRouter.post('/getGold', authenticationStep, getGoldController);

export default protectedRouter;
