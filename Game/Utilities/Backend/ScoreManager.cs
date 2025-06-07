using System.Collections.Generic;

namespace Game.Utilities.Backend
{
    public static class ScoreManager
    {
        // Score for each player (Key: PlayerId, Value: Score)
        public static Dictionary<long, int> PlayerScores { get; } = new();

        // Combo-System
        public static int ComboMultiplier { get; set; } = 1;
        public static float ComboTimer { get; set; } = 0f;
        public static float ComboTimeout { get; set; } = 2.0f; // Sekunden für Combo-Fenster

        public static void OnEnemyKilled(long playerId, int baseScore)
        {
            
            if (!PlayerScores.ContainsKey(playerId))
                PlayerScores[playerId] = 0;

            PlayerScores[playerId] += baseScore * ComboMultiplier;

            
            ComboMultiplier++;
            ComboTimer = ComboTimeout;
        }

        public static void UpdateCombo(float delta)
        {
            if (ComboMultiplier > 1)
            {
                ComboTimer -= delta;
                if (ComboTimer <= 0f)
                {
                    ComboMultiplier = 1;
                    ComboTimer = 0f;
                }
            }
        }
    }
}