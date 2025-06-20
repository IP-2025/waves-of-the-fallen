import { loginController, publicAccountDeleteController, registrateController } from 'controllers';
import { Router } from 'express';

const router = Router();

router.post('/login', loginController);

router.post('/register', registrateController);

router.post('/user/delete-account', publicAccountDeleteController)

export default router;
