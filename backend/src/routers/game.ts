import { listGameController, startGameController, stopGameController } from 'controllers';
import { Router } from 'express';

const gameRouter = Router();

gameRouter.post('/start', startGameController);

gameRouter.get('/all', listGameController);
gameRouter.post('/stop', stopGameController);

export default gameRouter;
