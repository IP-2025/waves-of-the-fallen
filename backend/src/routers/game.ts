import { registrateController, startGameController } from 'controllers';
import { Router } from 'express';

const gameRouter = Router();

gameRouter.post('/start', startGameController);

gameRouter.post('/all', registrateController);
gameRouter.post('/stop', registrateController);

export default gameRouter;
