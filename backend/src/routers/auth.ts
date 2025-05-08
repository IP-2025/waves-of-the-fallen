import { loginController } from 'controllers';
import { registrateController } from 'controllers';
import { Router } from 'express';

const router = Router();

router.post('/login', loginController);

router.post('/register', registrateController);

export default router;
