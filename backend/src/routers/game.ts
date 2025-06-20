import { joinGameController, listGameController, startGameController, stopGameController } from 'controllers';
import { Router } from 'express';

const gameRouter = Router();

gameRouter.post('/start', startGameController);
gameRouter.post('/join', joinGameController);

gameRouter.get('/all', listGameController);
gameRouter.post('/stop', stopGameController);

export default gameRouter;
