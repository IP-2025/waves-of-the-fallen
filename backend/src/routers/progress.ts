import express from 'express';
import { authenticationStep } from 'middleware';
import { getProgress, progressSyncController } from 'controllers';

const progressRouter = express.Router();

progressRouter.post('/', authenticationStep, getProgress);
progressRouter.post('/sync', authenticationStep, progressSyncController);

export default progressRouter;