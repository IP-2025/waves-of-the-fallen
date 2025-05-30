import express from 'express';
import { authenticationStep } from 'middleware';
import { getSettings, setSettings } from 'controllers';

const settingsRouter = express.Router();

settingsRouter.post('/get', authenticationStep, getSettings);
settingsRouter.post('/set', authenticationStep, setSettings);

export default settingsRouter;