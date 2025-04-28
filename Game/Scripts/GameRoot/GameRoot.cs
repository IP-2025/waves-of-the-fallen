using Godot;
using System;
using System.Data;

// GameRoot is the main entry point for the game. It is responsible for loading the map, spawning the player, starting the enemy spawner and so on.
public partial class GameRoot : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Engine.MaxFps = 60; // important! performance...

		// load map
		SpawnMap("res://Scenes/Main.tscn");

		// spawn player
		var peerId = GetTree().GetMultiplayer().GetUniqueId();
		SpawnPlayer(peerId);

		// add wave timer
		AddWaveTimer("res://Scenes/Waves/WaveTimer.tscn");

		// starting EnemySpawner
		SpawnEnemySpawner("res://Scenes/Enemies/SpawnEnemies.tscn");

		// start game on server
		NetworkManager.Instance.StartGame();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SpawnPlayer(long peerId)
	{
		var player = GD.Load<PackedScene>("res://Scenes/Characters/default_player.tscn").Instantiate<Node2D>();
		player.Name = peerId.ToString();
		player.Name = $"Player_{peerId}";
		// add joystick to player
		var joystick = GD.Load<PackedScene>("res://Scenes/Joystick/joystick.tscn").Instantiate<Node2D>();
		player.AddChild(joystick);
		AddChild(player);
		GameManager.Instance.Entities[peerId] = player;
	}

	public void SpawnMap(string mapPath)
	{
		var map = GD.Load<PackedScene>(mapPath).Instantiate<Node2D>();
		AddChild(map);
	}

	public void SpawnEnemySpawner(string enemySpawnerPath)
	{
		var spawner = GD.Load<PackedScene>(enemySpawnerPath).Instantiate<SpawnEnemies>();
		AddChild(spawner);
	}

	public void AddWaveTimer(string waveTimerPath)
	{
		var waveTimer = GD.Load<PackedScene>(waveTimerPath).Instantiate<WaveTimer>();
		AddChild(waveTimer);
	}
}
