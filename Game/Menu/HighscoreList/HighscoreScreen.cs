using Godot;
using System.Collections.Generic;

namespace Game.Menu.HighscoreList;

public partial class HighscoreScreen : Control
{
	public override void _Ready()
	{
		// Dummy-Daten
		var highscores = new List<Dictionary<string, object>>
		{
			new() { { "name", "Alice" }, { "score", 1000 } },
			new() { { "name", "Bob" }, { "score", 900 } },
			new() { { "name", "Charlie" }, { "score", 800 } },
			// ... weitere Dummy-Einträge
		};

		var entryScene = GD.Load<PackedScene>("res://Menu/HighscoreList/entry.tscn");
		var vbox = GetNode<VBoxContainer>("Panel/List/VBoxContainer");

		for (int i = 0; i < highscores.Count; i++)
		{
			var entry = entryScene.Instantiate<Control>();
			entry.GetNode<Label>("Position").Text = (i + 1).ToString();
			entry.GetNode<Label>("Name").Text = highscores[i]["name"].ToString();
			entry.GetNode<Label>("Score").Text = highscores[i]["score"].ToString();
			vbox.AddChild(entry);
		}
	}
}
