using Godot;
using System;
using System.Diagnostics;
using System.Linq;
using Game.Utilities.Multiplayer;

// GameRoot is the main entry point for the game. It is responsible for loading the map, spawning the player, starting the enemy spawner and so on.
public partial class GameRoot : Node
{
	private Node2D _mainMap;
	private int playerIndex = 0; // player index for spawning players
	bool isServer = false;
	private bool enableDebug = false;
	private WaveTimer globalWaveTimer;
		

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("_Ready() in GameRoot triggered");
		Engine.MaxFps = 60; // important! performance...

		isServer = GetTree().GetMultiplayer().IsServer();

		// Load map and store reference
		SpawnMap("res://Maps/GrassMap/Main.tscn");

		// Clients skip the rest – only the server continues
		if (!isServer) return;

		// Instantiate one global WaveTimer for server-wide access
		var waveTimerScene = GD.Load<PackedScene>("res://Utilities/Gameflow/Waves/WaveTimer.tscn");
		globalWaveTimer = waveTimerScene.Instantiate<WaveTimer>();
		globalWaveTimer.Name = "GlobalWaveTimer";
		AddChild(globalWaveTimer);

		// Server spawns all players
		foreach (var peerId in GetTree().GetMultiplayer().GetPeers())
		{
			GD.Print($">>> Spawning player with peer ID {peerId}");
			DebugIt($"Server spawning player {peerId}");
			SpawnPlayer(peerId);
		}

		// Start enemy spawner
		SpawnEnemySpawner("res://Utilities/Gameflow/Spawn/SpawnEnemies.tscn");
	}


	public override void _Process(double delta)
	{
		// Game logic per frame (optional)
	}
	
	public void OnSpectatorSwitchTarget(int direction)
	{
		// This method is called when the player presses the left or right button in Spectator UI
		// direction is -1 for left, +1 for right
		GD.Print($"Spectator wants to switch target. Direction: {direction}");
	}


	public void SpawnMap(string mapPath)
	{
		_mainMap = GD.Load<PackedScene>(mapPath).Instantiate<Node2D>();
		AddChild(_mainMap);
	}

	public void SpawnPlayer(long peerId)
	{
		int characterId = Server.Instance.PlayerSelections.ContainsKey(peerId)
		? Server.Instance.PlayerSelections[peerId]
		: -1;
		GD.Print($"[SpawnPlayer] Starting SpawnPlayer for peerId: {peerId}, characterId: {characterId}");


		// Load the correct character scene first
		DefaultPlayer player = null;

		//int characterId = Server.Instance.PlayerSelections[peerId];
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
		player.SetMultiplayerAuthority((int)peerId);
		GD.Print($"[SpawnPlayer] Set MultiplayerAuthority to {peerId} for player {player.Name}");
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
		
		Server.Instance.Entities[peerId] = player;

		// Try to find and connect the Health node signal (deep recursive)
		Health foundHealth = null;

		void FindHealthRecursive(Node node)
		{
			foreach (var child in node.GetChildren())
			{
				if (child is Health h)
				{
					foundHealth = h;
					return;
				}
				if (child is Node subNode)
					FindHealthRecursive(subNode);
			}
		}

		GD.Print($"[SpawnPlayer] Begin search for Health on Player_{peerId} (scene: {player.SceneFilePath})");
		FindHealthRecursive(player);

		if (foundHealth != null)
		{
			GD.Print($"[SpawnPlayer] Health node found on Player_{peerId}, connecting signal");
			foundHealth.HealthDepleted += () => OnPlayerDied(peerId);
			GD.Print($"[SpawnPlayer] Signal connected successfully for Player_{peerId}");
		}
		else
		{
			GD.PrintErr($"[SpawnPlayer] Health node NOT found on Player_{peerId}. Spectator UI will not trigger.");
		}

		playerIndex++;
	}

	public void SpawnEnemySpawner(string enemySpawnerPath)
	{
		var spawner = GD.Load<PackedScene>(enemySpawnerPath).Instantiate<SpawnEnemies>();
		AddChild(spawner);
	}

	public void OnPlayerDied(long deadPeerId)
	{
		GD.Print($"OnPlayerDied() triggered on node: {Name}, peer: {Multiplayer.GetUniqueId()}");
		
		// Prevent duplicate UI creation if already exists
		if (HasNode("SpectatorUI"))
		{
			GD.Print("Spectator UI already exists – skipping.");
			return;
		}

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

		long peerId = deadPeerId;
		
		// Trigger Spectator UI via player's node
		if (Multiplayer.IsServer())
		{
			if (Server.Instance.Entities.TryGetValue(peerId, out var playerNodeForRpc) && IsInstanceValid(playerNodeForRpc))
			{
				//GD.Print($"Calling RPC_ShowSpectatorUI on player node for peer {peerId}");
				var defaultPlayer = playerNodeForRpc as DefaultPlayer;
				//GD.Print($"[GameRoot] Sending RPC to Player Node: {playerNodeForRpc.Name}, OwnerPeer: {defaultPlayer?.OwnerPeerId}");

				RpcId((int)peerId, nameof(Rpc_ShowSpectatorUI));
				//GD.Print($"[GameRoot] Sent Rpc_ShowSpectatorUI via RpcId to peer {peerId}");


			}
			else
			{
				//GD.PrintErr($"[OnPlayerDied] Could not find player node for peer {peerId}");
			}
		}


		// Remove player entity to prevent a leftover copy
		if (Server.Instance.Entities.TryGetValue(peerId, out var playerNode))
		{
			if (IsInstanceValid(playerNode))
			{
				playerNode.CallDeferred("queue_free");
				//GD.Print($"Deferred removal of player node: Player_{peerId}");
			}

			Server.Instance.Entities.Remove(peerId);
		}

		// Find another player who is still alive
		var alivePlayers = GetTree()
			.GetNodesInGroup("player")
			.OfType<Node2D>()
			.Where(p => p.Name != $"Player_{peerId}")
			.ToList();

		if (alivePlayers.Count > 0)
		{
			var target = alivePlayers[0];
			var cam = target.GetNodeOrNull<Camera2D>("Camera2D");
			if (cam != null)
			{
				cam.MakeCurrent();
				GD.Print($"Camera switched to alive player: {target.Name}");
			}
		}
		else
		{
			GD.Print("No alive players left.");
		}
	}

	private void DebugIt(string message)
	{
		if (enableDebug) Debug.Print("GameRoot: " + message);
	}
	
	
	[Rpc(MultiplayerApi.RpcMode.Authority)]
	public void Rpc_ShowSpectatorUI()
	{
		GD.Print($"[GameRoot] Rpc_ShowSpectatorUI triggered on peer {Multiplayer.GetUniqueId()}");

		if (HasNode("SpectatorUI"))
		{
			GD.Print("Spectator UI already exists – skipping.");
			return;
		}

		var uiScene = GD.Load<PackedScene>("res://UI/Spectator/Spectator.tscn");
		var uiInstance = uiScene.Instantiate<Spectator>();
		uiInstance.Name = "SpectatorUI";
		
		var currentCamera = GetViewport().GetCamera2D();
		
		AddChild(uiInstance);
		GD.Print("Spectator UI (CanvasLayer) successfully added to GameRoot.");

		uiInstance.SetPlayerName($"Player {Multiplayer.GetUniqueId()}");
		uiInstance.EnableUI(true);

		GD.Print("Spectator UI successfully added.");
	}

	
}
