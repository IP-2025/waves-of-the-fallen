import {AppDataSource} from "database/dataSource";
import {HighScore} from "../database/entities";
import {NotFoundError} from "../errors";

const highScoreRepo = AppDataSource.getRepository(HighScore)


export async function updateHighScoreRepo(playerId: string, highScore: number): Promise<void> {
    const existing = await highScoreRepo.findOne({
        where: { player: { player_id: playerId } }
    });

    if (existing) {
        const result = await highScoreRepo.update({ player: { player_id: playerId } }, { highScore });
        if (result.affected === 0) {
            throw new NotFoundError('Player not found');
        }
    } else {
        await highScoreRepo.insert({
            player: { player_id: playerId },
            highScore
        });
    }
}

export async function getUserHighscoreRepo(playerId: string): Promise<number> {
    const highScore = await highScoreRepo.findOne({
        where: { player: { player_id: playerId } }
    });

    if (!highScore) {
        throw new NotFoundError('Player not found');
    }

    return highScore.highScore;
}

export async function getTopHighscoreRepo(): Promise<HighScore[]> {
    const highScores = await highScoreRepo.find({
        order: { highScore: "DESC" },
        relations: ["player"],
        take: 100,
    });

    if (!highScores) {
        throw new NotFoundError('No top users');
    }

    return highScores;
}
