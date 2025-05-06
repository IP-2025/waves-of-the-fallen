import { loginController } from '../controllers/loginController';
import { registrationController } from '../controllers/registerController';
import { Router } from 'express';

const router = Router();

router.post('/login', loginController);

router.post('/register', registrationController);

export default router;
