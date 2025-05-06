import express from 'express';
import { authenticationStep } from '../middleware/validateMiddleware';
import { getSettings, setSettings } from '../controllers/settingsController';
import { getAllCharacterController, getAllUnlockedCharacterController } from '../controllers/characterController';

const protectedRouter = express.Router();

protectedRouter.get('/', authenticationStep, (req, res) => {
  res.json({ authenticated: true });
});
protectedRouter.post('/getSettings', authenticationStep, getSettings);
protectedRouter.post('/setSettings', authenticationStep, setSettings);
//TODO : add authentication to this route (Just for Local Testing)
protectedRouter.post('/getAllCharacters',getAllCharacterController);
protectedRouter.post('/getAllUnlockedCharacters',getAllUnlockedCharacterController);


export default protectedRouter;
