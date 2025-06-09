import express from 'express';
import { authenticationStep } from 'middleware';
import {deletePlayerController, getProgress, progressSyncController} from 'controllers';

const userRouter = express.Router();

userRouter.delete('/', authenticationStep, deletePlayerController);

export default userRouter;