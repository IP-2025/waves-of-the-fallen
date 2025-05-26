import express from 'express';
import { authenticationStep } from 'middleware';
import { updateUserHighscore, getUserHighscore, getTopHighscore } from 'controllers';

const highscoreRouter = express.Router();

highscoreRouter.post('/update', authenticationStep, updateUserHighscore);
highscoreRouter.post('/getUserHighscore', authenticationStep, getUserHighscore);
highscoreRouter.get('/top', authenticationStep, getTopHighscore);

export default highscoreRouter;