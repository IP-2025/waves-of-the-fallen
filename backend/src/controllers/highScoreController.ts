import {NextFunction} from "express";
import {extractAndValidatePlayerId} from "auth/jwt";
import {BadRequestError} from "errors";
import {getTopHighscoreService, getUserHighscoreService, isBiggerThanExistingScore} from "services";
import {updateHighScoreService} from "services";

export async function updateUserHighscore(req: any, res: any, next: NextFunction) {
    try {

        const {score} = req.body;
        const playerId = extractAndValidatePlayerId(req.headers['authorization']);

        if (!playerId || !score) {
            throw new BadRequestError('Missing required fields');
        }
        if (score < 0) {
            throw new BadRequestError('Score cannot be negative');
        }

        const bigger = await isBiggerThanExistingScore(playerId, score);
        if (bigger) {
            await updateHighScoreService(playerId, score);
        }

        res.status(200).json({message: 'Highscore updated successfully'});
    } catch (err) {
        next(err)
    }
}


export async function getUserHighscore(req: any, res: any, next: NextFunction) {
    try {
        const playerId = extractAndValidatePlayerId(req.headers['authorization']);
        if (!playerId) {
            throw new BadRequestError('Missing required fields');
        }
        const highScore = await getUserHighscoreService(playerId);

        res.status(200).json({highScore});
    } catch (err) {
        next(err)
    }


}

export async function getTopHighscore(req: any, res: any, next: NextFunction) {
    try {
       const highScoreList = await getTopHighscoreService();
       res.status(200).json({highScoreList});
    } catch (err) {
        next(err)
    }
}