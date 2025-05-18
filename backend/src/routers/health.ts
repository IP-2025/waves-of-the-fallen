import { Router } from 'express';
import { healthCheck } from 'controllers';

const healtRouter = Router();

healtRouter.get('/', healthCheck);

export default healtRouter;
