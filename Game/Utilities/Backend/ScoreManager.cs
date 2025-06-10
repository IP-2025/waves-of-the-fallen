using System.Collections.Generic;
using Godot;

namespace Game.Utilities.Backend
{
	public static class ScoreManager
	{
		// Score for each player (Key: PlayerId, Value: Score)
		public static Dictionary<long, int> PlayerScores { get; } = new();

		// Combo-System for each player
		public static Dictionary<long, int> ComboMultipliers { get; } = new();
		public static Dictionary<long, float> ComboTimers { get; } = new();
		public static float ComboTimeout { get; set; } = 2.0f;

		public static readonly List<Color> PlayerColorList = new()
		{
			new Color(1, 0f, 0f),       // red
			new Color(0.1f, 0.4f, 1),   // bright blue
			new Color(1, 0.7f, 0.1f),   // bright orange-yellow
			new Color(0.7f, 0.2f, 1),   // bright purple
		};

		public static void OnEnemyKilled(long playerId, int baseScore)
		{
			if (!PlayerScores.ContainsKey(playerId))
				PlayerScores[playerId] = 0;
			if (!ComboMultipliers.ContainsKey(playerId))
				ComboMultipliers[playerId] = 1;
			if (!ComboTimers.ContainsKey(playerId))
				ComboTimers[playerId] = 0f;

			PlayerScores[playerId] += baseScore * ComboMultipliers[playerId];

			if (ComboTimers[playerId] > 0f)
				ComboMultipliers[playerId]++;
			else
				ComboMultipliers[playerId] = 1;

			ComboTimers[playerId] = ComboTimeout;
		}

		public static void UpdateCombo(long playerId, float delta)
		{
			if (!ComboMultipliers.ContainsKey(playerId) || ComboMultipliers[playerId] <= 1)
				return;

			ComboTimers[playerId] -= delta;
			if (ComboTimers[playerId] <= 0f)
			{
				ComboMultipliers[playerId] = 1;
				ComboTimers[playerId] = 0f;
			}
		}

		public static int GetCombo(long playerId)
		{
			return ComboMultipliers.ContainsKey(playerId) ? ComboMultipliers[playerId] : 1;
		}

		public static void AddBonus(long playerId, int bonus)
		{
			if (!PlayerScores.ContainsKey(playerId))
				PlayerScores[playerId] = 0;
			PlayerScores[playerId] += bonus;
		}

		public static Color GetPlayerColor(long playerId)
		{
			if (playerId >= 1 && playerId <= PlayerColorList.Count)
				return PlayerColorList[(int)playerId - 1];
			return PlayerColorList[(int)(playerId % PlayerColorList.Count)];
		}

		public static void Reset()
		{
			PlayerScores.Clear();
			ComboMultipliers.Clear();
			ComboTimers.Clear();
		}
	}
}
