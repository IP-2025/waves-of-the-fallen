using Godot;
using Game.Utilities.Backend;
using Game.Utilities.Multiplayer;
using System.Text;

public partial class HUD : CanvasLayer
{
	public override void _Process(double delta)
	{
		var sb = new StringBuilder();

		if (NetworkManager.Instance != null && NetworkManager.Instance._soloMode)
		{
			// Solo-Mode: score of the local player
			long peerId = Multiplayer.GetUniqueId();
			int score = ScoreManager.PlayerScores.ContainsKey(peerId) ? ScoreManager.PlayerScores[peerId] : 0;
			var playerNode = GetTree().Root.GetNodeOrNull<Node>("GameRoot")?.GetNodeOrNull<Node>($"Player_{peerId}");
			string className = playerNode != null ? playerNode.GetType().Name : $"Player {peerId}";
			sb.AppendLine($"{className}: {score}");
		}
		else
		{
			// Multiplayer: score of all players
			foreach (var kv in ScoreManager.PlayerScores)
			{
				string playerNodeName = $"E_{kv.Key}";
				var playerNode = GetTree().Root.GetNodeOrNull<Node2D>("GameRoot")?.GetNodeOrNull<Node2D>(playerNodeName);
				string className = playerNode != null ? playerNode.GetType().Name : $"Player {kv.Key}";
				sb.AppendLine($"{className}: {kv.Value}");
			}
		}

		sb.AppendLine($"Combo: x{ScoreManager.ComboMultiplier}");

		var label = GetNodeOrNull<Label>("ScoreLabel");
		if (label == null)
			GD.PrintErr("ScoreLabel not found!");
		else
			label.Text = sb.ToString();
	}
}
