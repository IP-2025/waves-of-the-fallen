using Godot;
using System;

public partial class Main : Node2D
{
	public bool enableDebug = false;
	[Export]
	private PackedScene playerScene = (PackedScene)
		ResourceLoader.Load("res://Scenes/Characters/default_player.tscn");

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Engine.MaxFps = 60; // important! performance...

		int index = 0;
		foreach (var item in GameManager.Players) // adding all players to the game / map
		{
			DebugIt(("Name: ", item.Name, " Multiplayer UniqueId: ", item.Id).ToString());
			DefaultPlayer currentPlayer = playerScene.Instantiate<DefaultPlayer>(); // spawning player
																					//player.Joystick = GetNode<Node2D>("CanvasLayer/Joystick");
			currentPlayer.Name = item.Id.ToString(); // id is unique so it should be the name
			AddChild(currentPlayer); // add it to the world as child node

			// set players spawn pos to spawnpoint pos
			foreach (Node2D spawnPoint in GetTree().GetNodesInGroup("PlayerSpawnPoints"))
			{
				if (int.Parse(spawnPoint.Name) == index)
				{
					currentPlayer.GlobalPosition = spawnPoint.GlobalPosition;
					break;
				}
			}
			index++; // next player
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void DebugIt(string message)
	{
		if (enableDebug)
		{
			GD.Print(message);
		}
	}
}
