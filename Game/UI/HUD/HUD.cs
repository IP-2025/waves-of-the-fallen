using Godot;
using Game.Utilities.Backend;
using Game.Utilities.Multiplayer;
using System.Text;
using System.Collections.Generic;

public partial class HUD : CanvasLayer
{
	public override void _Process(double delta)
	{
		var sb = new StringBuilder();
		long peerId = Multiplayer.GetUniqueId(); 

		if (NetworkManager.Instance != null && NetworkManager.Instance._soloMode)
		{
			// Solo-Mode: score of the local player
			int score = ScoreManager.PlayerScores.ContainsKey(peerId) ? ScoreManager.PlayerScores[peerId] : 0;
			var playerNode = GetTree().Root.GetNodeOrNull<Node>("GameRoot")?.GetNodeOrNull<Node>($"Player_{peerId}");
			string className = playerNode != null ? playerNode.GetType().Name : $"Player {peerId}";
			var color = ScoreManager.GetPlayerColor(peerId);
			string colorHex = color.ToHtml();
			sb.AppendLine($"[color={colorHex}]{className}[/color]: {score}");
		}
		else
		{
			// Multiplayer: score of all players
			foreach (var kv in ScoreManager.PlayerScores)
			{
				var color = ScoreManager.GetPlayerColor(kv.Key);
				string colorHex = color.ToHtml();
				var playerNode = GetTree().Root.GetNodeOrNull<Node>("GameRoot")?.GetNodeOrNull<Node>($"Player_{kv.Key}");
				string className = $"[color={colorHex}]{(playerNode != null ? playerNode.GetType().Name : $"Player {kv.Key}")}[/color]";
				int combo = ScoreManager.GetCombo(kv.Key);
				sb.AppendLine($"{className}: {kv.Value} (Combo: x{combo})");
			}
		}

		var label = GetNodeOrNull<RichTextLabel>("ScoreLabel");
		if (label == null)
			GD.PrintErr("ScoreLabel not found!");
		else
		{
			label.BbcodeEnabled = true;
			label.Text = sb.ToString();
		}
	}
}
