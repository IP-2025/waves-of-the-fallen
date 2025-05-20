import {getTopHighscoreRepo, getUserHighscoreRepo, updateHighScoreRepo} from 'repositories/highScoreRepository';
import { termLogger as logger } from 'logger';
import {HighScore} from "../database/entities";


export async function isBiggerThanExistingScore(playerId: string, newScore: number): Promise<boolean>  {
    try {
        const oldScore = await getUserHighscoreRepo(playerId)
        return newScore > oldScore;
    }
    catch (error) {
        logger.info("Highscore does not exist for user, creating new highscore");
        return true;
    }

}

export async function updateHighScoreService(playerId: string, highScore: number): Promise<void> {
    await updateHighScoreRepo(playerId, highScore);
}

export  async  function  getUserHighscoreService(playerId: string){
    await getUserHighscoreRepo(playerId);
}

export  async  function  getTopHighscoreService(): Promise<HighScore[]> {
    return await getTopHighscoreRepo();
}