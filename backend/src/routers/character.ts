import express from 'express';
import { authenticationStep } from 'middleware';
import { getAllCharacterController, unlockCharController, levelUpCharController } from 'controllers';

const characterRouter = express.Router();

characterRouter.get('/', authenticationStep, getAllCharacterController);
characterRouter.post('/unlock', authenticationStep, unlockCharController);
characterRouter.post('/levelUp', authenticationStep, levelUpCharController);

export default characterRouter;