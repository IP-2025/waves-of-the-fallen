import { loginController } from '../controllers/loginController';
import { registrateController } from '../controllers/registerController';
import { Router } from 'express';
import {startGameController} from "../controllers/startGameController";

const router = Router();

router.post('/start', startGameController);

router.post('/all', registrateController);
router.post('/stop', registrateController);

export default router;
