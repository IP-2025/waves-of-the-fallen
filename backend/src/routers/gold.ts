import express from 'express';
import { authenticationStep } from 'middleware';
import { getGoldController, setGoldController, addGoldController } from 'controllers';

const goldRouter = express.Router();

goldRouter.post('/get', authenticationStep, getGoldController);
goldRouter.post('/set', authenticationStep, setGoldController);
goldRouter.post('/add', authenticationStep, addGoldController);

export default goldRouter;