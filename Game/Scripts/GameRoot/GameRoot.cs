using Godot;
using System;
using System.Diagnostics;
using System.Linq;

// GameRoot is the main entry point for the game. It is responsible for loading the map, spawning the player, starting the enemy spawner and so on.
public partial class GameRoot : Node
{
	private Node2D _mainMap;
	private int playerIndex = 0; // player index for spawning players
	bool isServer = false;
	private bool enableDebug = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Engine.MaxFps = 60; // important! performance...

		isServer = GetTree().GetMultiplayer().IsServer();

		// Load map and store reference
		SpawnMap("res://Scenes/Main.tscn");

		// spawn players
		if (isServer)
		{
			// Server spawns all players
			foreach (var peerId in GetTree().GetMultiplayer().GetPeers())
			{
				DebugIt($"Server spawning player {peerId}");
				SpawnPlayer(peerId);
			}

			// Start enemy spawner
			SpawnEnemySpawner("res://Scenes/Enemies/SpawnEnemies.tscn");

			// Add wave timer
			AddWaveTimer("res://Scenes/Waves/WaveTimer.tscn");
		}
		else
		{
			/* 			// Client spawns only itself
						SpawnPlayer(GetTree().GetMultiplayer().GetUniqueId());
						DebugIt("Client spawning player"); */
		}
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

		// Get spawn point from PlayerSpawnPoints group
		player.GlobalPosition = GetTree().GetNodesInGroup("PlayerSpawnPoints")
			.OfType<Node2D>()
			.ToList()
			.FindAll(spawnPoint => int.Parse(spawnPoint.Name) == playerIndex)
			.FirstOrDefault()?.GlobalPosition ?? Vector2.Zero;

		// Add joystick to player
		var joystick = GD.Load<PackedScene>("res://Scenes/Joystick/joystick.tscn").Instantiate<Node2D>();
		player.AddChild(joystick);

		AddChild(player);
		Server.Instance.Entities[peerId] = player;
		
		// Set the player's camera as current if this is the local player
		if (GetTree().GetMultiplayer().GetUniqueId() == peerId)
		{
			var camera = player.GetNode<Camera2D>("Camera2D");
			camera.MakeCurrent();
		}

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

		playerIndex++;
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
		DebugIt("Player died! Showing Game Over screen.");

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

	private void DebugIt(string message)
	{
		if (enableDebug) Debug.Print("GameRoot: " + message);
	}
}
