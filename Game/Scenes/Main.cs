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
			
			/// <summary>
			/// Instantiates the Mage class directly as a placeholder due to the absence of a class selection menu
			/// This is a temporary solution to test gameplay with the Mage class
			/// To test a different class, replace 'Mage' with the desired class name
			/// </summary>
			var mage = new Mage(); // Instantiate Mage
			currentPlayer.Speed = mage.Speed; // Override DefaultPlayer's Speed with Mage's Speed
			currentPlayer.MaxHealth = mage.MaxHealth; // Override DefaultPlayer's MaxHealth with Mage's MaxHealth
			currentPlayer.CurrentHealth = mage.CurrentHealth; // Set CurrentHealth to Mage's CurrentHealth
			GD.Print($"Mage Speed applied: {currentPlayer.Speed}, Mage Health applied: {currentPlayer.MaxHealth}");
			
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
