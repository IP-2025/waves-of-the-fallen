using Godot;
using System;
using System.Data;
using System.Globalization;
using System.Linq;

// GameRoot is the main entry point for the game. It is responsible for loading the map, spawning the player, starting the enemy spawner and so on.
public partial class GameRoot : Node
{
	// Called when the node enters the scene tree for the first time.

	private int playerIndex = 0; // player index for spawning players
	public override void _Ready()
	{
		// setup vor all defauld and consitent stuff like map, players and so on.. inconsistent stuff like
		// enemie position and so on are handled by snapshots.

		Engine.MaxFps = 60; // important! performance...

		// load map
		SpawnMap("res://Scenes/Main.tscn");

		// starting EnemySpawner
		SpawnEnemySpawner("res://Scenes/Enemies/SpawnEnemies.tscn");

		// spawn players for all peers
		foreach (var peer in GetTree().GetMultiplayer().GetPeers())
		{
			SpawnPlayer(peer);
		}

		BuildHud();
	}

	private void SpawnMap(string mapPath)
	{
		var map = GD.Load<PackedScene>(mapPath).Instantiate<Node2D>();
		AddChild(map);
	}

	private void SpawnEnemySpawner(string enemySpawnerPath)
	{
		var spawner = GD.Load<PackedScene>(enemySpawnerPath).Instantiate<SpawnEnemies>();
		AddChild(spawner);
	}

	private void AddWaveTimer(string waveTimerPath)
	{
		var waveTimer = GD.Load<PackedScene>(waveTimerPath).Instantiate<WaveTimer>();
		AddChild(waveTimer);
	}

	private void BuildHud()
	{
		// add wave timer
		AddWaveTimer("res://Scenes/Waves/WaveTimer.tscn");
	}


	public void SpawnPlayer(long peerId)
	{
		var player = GD.Load<PackedScene>("res://Scenes/Characters/default_player.tscn").Instantiate<Node2D>();
		player.Name = $"Player_{peerId}";

		// Get spawn point from PlayerSpawnPoints group
		player.GlobalPosition = GetTree().GetNodesInGroup("PlayerSpawnPoints")
			.OfType<Node2D>()
			.ToList()
			.FindAll(spawnPoint => int.Parse(spawnPoint.Name) == playerIndex)
			.FirstOrDefault()?.GlobalPosition ?? Vector2.Zero;
		player.AddChild(GD.Load<PackedScene>("res://Scenes/Joystick/joystick.tscn").Instantiate<Node2D>());
		AddChild(player);
		Server.Instance.Entities[peerId] = player;

		playerIndex++;
	}

}
