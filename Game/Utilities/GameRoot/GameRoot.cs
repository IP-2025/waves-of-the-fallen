using Godot;
using System;
using System.Diagnostics;
using System.Linq;
using Game.Utilities.Multiplayer;
using Game.UI.GameOver;

// GameRoot is the main entry point for the game. It is responsible for loading the map, spawning the player, starting the enemy spawner and so on.
public partial class GameRoot : Node
{
	private Node2D _mainMap;
	private int playerIndex = 0; // player index for spawning players
	bool isServer = false;
	private bool enableDebug = false;
	private WaveTimer globalWaveTimer;
	private GameOverScreen _gameOverScreen;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Engine.MaxFps = 60; // important! performance...

		isServer = GetTree().GetMultiplayer().IsServer();

		// Load map and store reference
		SpawnMap("res://Maps/GrassMap/Main.tscn");

		// Instantiate one global WaveTimer for server-wide access
		if (isServer)
		{
			var waveTimerScene = GD.Load<PackedScene>("res://Utilities/Gameflow/Waves/WaveTimer.tscn");
			globalWaveTimer = waveTimerScene.Instantiate<WaveTimer>();
			globalWaveTimer.Name = "GlobalWaveTimer";
			AddChild(globalWaveTimer);
		}
		if (isServer)
		{
			// Server spawns all players
			foreach (var peerId in GetTree().GetMultiplayer().GetPeers())
			{
				DebugIt($"Server spawning player {peerId}");
				SpawnPlayer(peerId);
			}

			// Start enemy spawner
			SpawnEnemySpawner("res://Utilities/Gameflow/Spawn/SpawnEnemies.tscn");
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
		var player = GD.Load<PackedScene>("res://Entities/Characters/Mage/mage.tscn").Instantiate<DefaultPlayer>();
		player.OwnerPeerId = peerId;
		player.Name = $"Player_{peerId}";

		

		int characterId = Server.Instance.PlayerSelections[peerId];
		switch (characterId)
		{
			case 1:
				player = GD.Load<PackedScene>("res://Entities/Characters/Archer/archer.tscn").Instantiate<DefaultPlayer>();
				break;

			case 2:
				player = GD.Load<PackedScene>("res://Entities/Characters/Assassin/assassin.tscn").Instantiate<DefaultPlayer>();
				break;

			case 3:
				player = GD.Load<PackedScene>("res://Entities/Characters/Knight/knight.tscn").Instantiate<DefaultPlayer>();
				break;

			case 4:
				player = GD.Load<PackedScene>("res://Entities/Characters/Mage/mage.tscn").Instantiate<DefaultPlayer>();
				break;

			default:
				break;
		}

			player.OwnerPeerId = peerId;
			player.Name = $"Player_{peerId}";


			// Get spawn point from PlayerSpawnPoints group
			player.GlobalPosition = GetTree().GetNodesInGroup("PlayerSpawnPoints")
				.OfType<Node2D>()
				.ToList()
				.FindAll(spawnPoint => int.Parse(spawnPoint.Name) == playerIndex)
				.FirstOrDefault()?.GlobalPosition ?? Vector2.Zero;

			// Add joystick to player
			var joystick = GD.Load<PackedScene>("res://UI/Joystick/joystick.tscn").Instantiate<Node2D>();
			player.AddChild(joystick);
			player.Joystick = joystick;
			var WaveTimer = GD.Load<PackedScene>("res://Utilities/Gameflow/Waves/WaveTimer.tscn").Instantiate<WaveTimer>();
			player.GetNode<Camera2D>("Camera2D").AddChild(WaveTimer);
			AddChild(player);
			player.AddToGroup("player");


			Server.Instance.Entities[peerId] = player;

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

	public void SpawnEnemySpawner(string enemySpawnerPath)
	{
		var spawner = GD.Load<PackedScene>(enemySpawnerPath).Instantiate<SpawnEnemies>();
		AddChild(spawner);
	}

	public void OnPlayerDied()
	{
		DebugIt("Player died!");

		// Ermittele PeerId durch Vergleich mit Entity-Tabelle
		long deadPeerId = -1;
		Node toRemove = null;

		foreach (var kv in Server.Instance.Entities)
		{
			if (kv.Value.GetNodeOrNull<Health>("Health")?.health <= 0)
			{
				deadPeerId = kv.Key;
				toRemove = kv.Value;
				break;
			}
		}

		if (deadPeerId != -1 && toRemove != null)
		{
			toRemove.QueueFree();
			Server.Instance.Entities.Remove(deadPeerId);
			DebugIt($"Removed player {deadPeerId}");
		}

		// Lebende Spieler aus Gruppe "player"
		var alivePlayers = GetTree()
			.GetNodesInGroup("player")
			.OfType<Node2D>()
			.Where(p => IsInstanceValid(p))
			.ToList();

		if (alivePlayers.Count > 0)
		{
			var target = alivePlayers[0];
			var cam = target.GetNodeOrNull<Camera2D>("Camera2D");
			if (cam != null)
			{
				cam.MakeCurrent();
				DebugIt($"Camera switched to alive player: {target.Name}");
			}
		}
		else
		{
			DebugIt("No alive players left. Showing Game Over screen.");

			if (_gameOverScreen == null)
			{
				var scene = GD.Load<PackedScene>("res://UI/GameOver/gameOverScreen.tscn");
				_gameOverScreen = scene.Instantiate<GameOverScreen>();
				AddChild(_gameOverScreen);
			}

			_gameOverScreen.ShowGameOver(0); // Score ggf. später ersetzen
		}
	}

private void DebugIt(string message)
{
	if (enableDebug) Debug.Print("GameRoot: " + message);
}
}
