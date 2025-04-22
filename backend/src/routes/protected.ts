import express from 'express';
import { authenticationStep } from '../middleware/validateMiddleware';
import { getSettings, setSettings } from '../controllers/settingsController';
import {getAllCharacters} from '../services/characterService';

const protectedRouter = express.Router();

protectedRouter.post('/getSettings', authenticationStep, getSettings);
protectedRouter.post('/setSettings', authenticationStep, setSettings);
protectedRouter.post('/getAllCharacters', authenticationStep,getAllCharacters);
protectedRouter.post('/getAllUnlockedCharacters', authenticationStep,getAllUnlockedCharacters);

protectedRouter.get('/', authenticationStep, (req, res) => {
  res.json({ authenticated: true });
});

export default protectedRouter;
