using Godot;
using System;

// GameRoot is the main entry point for the game. It is responsible for loading the map, spawning the player, starting the enemy spawner and so on.
public partial class GameRoot : Node
{
	private Node2D _mainMap;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Engine.MaxFps = 60; // important! performance...

		// Load map and store reference
		SpawnMap("res://Scenes/Main.tscn");

		// Spawn player
		var peerId = GetTree().GetMultiplayer().GetUniqueId();
		SpawnPlayer(peerId);

		// Add wave timer
		AddWaveTimer("res://Scenes/Waves/WaveTimer.tscn");

		// Start enemy spawner
		SpawnEnemySpawner("res://Scenes/Enemies/SpawnEnemies.tscn");

		// Start game on server
		NetworkManager.Instance.StartGame();
	}

	public override void _Process(double delta)
	{
		// Game logic per frame (optional)
	}

	public void SpawnMap(string mapPath)
	{
		_mainMap = GD.Load<PackedScene>(mapPath).Instantiate<Node2D>();
		AddChild(_mainMap);
	}

	public void SpawnPlayer(long peerId)
	{
		var player = GD.Load<PackedScene>("res://Scenes/Characters/default_player.tscn").Instantiate<Node2D>();
		player.Name = $"Player_{peerId}";

		// Add joystick to player
		var joystick = GD.Load<PackedScene>("res://Scenes/Joystick/joystick.tscn").Instantiate<Node2D>();
		player.AddChild(joystick);
		
		AddChild(player);
		GameManager.Instance.Entities[peerId] = player;

		// Connect health signal
		var healthNode = player.GetNodeOrNull<Health>("Health");
		if (healthNode != null)
		{
			healthNode.Connect(Health.SignalName.HealthDepleted, new Callable(this, nameof(OnPlayerDied)));
		}
		else
		{
			GD.PrintErr("Health node not found on player!");
		}
	}

	public void AddWaveTimer(string waveTimerPath)
	{
		var waveTimer = GD.Load<PackedScene>(waveTimerPath).Instantiate<WaveTimer>();
		AddChild(waveTimer);
	}

	public void SpawnEnemySpawner(string enemySpawnerPath)
	{
		var spawner = GD.Load<PackedScene>(enemySpawnerPath).Instantiate<SpawnEnemies>();
		AddChild(spawner);
	}

	public void OnPlayerDied()
	{
		GD.Print("Player died! Showing Game Over screen.");

		if (_mainMap == null)
		{
			GD.PrintErr("Main map is null!");
			return;
		}

		var gameOverScreen = _mainMap.GetNodeOrNull<CanvasLayer>("GameOver");
		if (gameOverScreen != null)
		{
			gameOverScreen.Visible = true;
			//GetTree().Paused = true;
		}
		else
		{
			GD.PrintErr("GameOver screen not found in main map!");
		}
	}
}
