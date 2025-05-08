import { loginController } from '../controllers/loginController';
import { registrateController } from '../controllers/registerController';
import { Router } from 'express';

const router = Router();

router.post('/login', loginController);

router.post('/register', registrateController);

export default router;
