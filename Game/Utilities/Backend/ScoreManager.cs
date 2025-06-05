using System.Collections.Generic;

namespace Game.Utilities.Backend
{
    public static class ScoreManager
    {
        // Score for each player (Key: PlayerId, Value: Score)
        public static Dictionary<long, int> PlayerScores { get; } = new();
    }
}