using Godot;
using Game.Utilities.Backend;
using System.Text;

public partial class HUD : CanvasLayer
{
	public override void _Process(double delta)
	{
		var sb = new StringBuilder();

		foreach (var kv in ScoreManager.PlayerScores)
		{
			string playerNodeName = $"E_{kv.Key}";
			var playerNode = GetTree().Root.GetNodeOrNull<Node2D>("GameRoot")?.GetNodeOrNull<Node2D>(playerNodeName);

			string className = playerNode != null ? playerNode.GetType().Name : $"Player {kv.Key}";
			sb.AppendLine($"{className}: {kv.Value}");
		}

		GetNode<Label>("ScoreLabel").Text = sb.ToString();
	}
}
