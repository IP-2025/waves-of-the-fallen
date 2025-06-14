using Godot;
using System.Diagnostics;
using System.Linq;
using Game.Utilities.Multiplayer;
using Game.UI.GameOver;
using Game.Utilities.Backend;
using Godot.Collections;

// GameRoot is the main entry point for the game. It is responsible for loading the map, spawning the player, starting the enemy spawner and so on.
public partial class GameRoot : Node
{
	private bool _enableDebug = true;
	private Node2D _mainMap;
	private int _playerIndex = 0; // player index for spawning players
	private bool _isServer = false;
	private WaveTimer _globalWaveTimer;
	private DefaultPlayer _soloPlayer;

	// Shop dirty workaround
	private int _lastLocalShopRound = 1;
	private Node _shopInstance;
	private int _newWeaponPos = 0;
	private string _selectedWeapon = "";

	// GameOver
	private GameOverScreen _gameOverScreen;


	private HttpRequest _sendScoreRequest;
	private bool _soloMode = false;
	private PauseMenu _pauseMenu;
	private CharacterManager characterManager;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready() // builds the game
	{
		ScoreManager.Reset();
		GetTree().Paused = false;

		// Score -------------------------------------------------------------------------------------------
		_sendScoreRequest = GetNodeOrNull<HttpRequest>("%SendScoreRequest");
		if (_sendScoreRequest == null)
		{
			GD.PrintErr("SendScoreRequest node not found! Please ensure it is present in the scene.");
			return;
		}
		_sendScoreRequest.Connect(HttpRequest.SignalName.RequestCompleted, new Callable(this, nameof(OnScoreRequestCompleted)));

		// Character ---------------------------------------------------------------------------------------
		characterManager = GetNode<CharacterManager>("/root/CharacterManager");

		if (!NetworkManager.Instance.SoloMode) _isServer = GetTree().GetMultiplayer().IsServer();

		// Map ---------------------------------------------------------------------------------------------
		// Load map and store reference
		_mainMap = GD.Load<PackedScene>("res://Maps/GrassMap/Main.tscn").Instantiate<Node2D>();
		AddChild(_mainMap);

		// HUD --------------------------------------------------------------------------------------------
		CanvasLayer hud = null;
		if (GetNodeOrNull<CanvasLayer>("HUD") == null)
		{
			var hudScene = GD.Load<PackedScene>("res://UI/HUD/HUD.tscn");
			hud = hudScene.Instantiate<CanvasLayer>();
			AddChild(hud);
		}
		// -------------------------------------------------------------------------------------------------
		if (!_isServer && !NetworkManager.Instance.SoloMode) return; // clients return

		// WaveTimer ---------------------------------------------------------------------------------------
		// Instantiate one global WaveTimer for server-wide access
		var waveTimerScene = GD.Load<PackedScene>("res://Utilities/Gameflow/Waves/WaveTimer.tscn");
		_globalWaveTimer = waveTimerScene.Instantiate<WaveTimer>();
		if (_isServer)
		{
			_globalWaveTimer.Name = "GlobalWaveTimer"; // is for sync with clients
			_globalWaveTimer.Visible = false; // or would spawn somewhere on the map
			AddChild(_globalWaveTimer);
			// Players -------------------------------------------------------------------------------------
			// spawn player for self
			if (NetworkManager.Instance._isLocalHost)
			{
				var localWaveTimer = waveTimerScene.Instantiate<WaveTimer>();
				localWaveTimer.Disable = true;
				SpawnPlayer(1);
				GetNodeOrNull<DefaultPlayer>("Player_1")
					.GetNodeOrNull<Camera2D>("Camera2D")
					.AddChild(localWaveTimer); // player needs wave timer, is normaly added in Client.cs
			}                                                       // Server spawns all players for peers
			foreach (var peerId in GetTree().GetMultiplayer().GetPeers())
			{
				DebugIt($"Server spawning player {peerId}");
				SpawnPlayer(peerId);
			}
		}

		// SoloMode ----------------------------------------------------------------------------------------
		if (NetworkManager.Instance.SoloMode)
		{
			ScoreManager.PlayerScores.TryAdd(1, 0);

			SpawnPlayer(1);
			_soloPlayer = GetNodeOrNull<DefaultPlayer>($"Player_1");
			GetNodeOrNull<DefaultPlayer>("Player_1")
				.GetNodeOrNull<Camera2D>("Camera2D")
				.AddChild(_globalWaveTimer);
		}

		// Enemies ----------------------------------------------------------------------------------------
		// Start enemy spawner
		var spawner = GD.Load<PackedScene>("res://Utilities/Gameflow/Spawn/SpawnEnemies.tscn").Instantiate<SpawnEnemies>();
		AddChild(spawner);

		// HUD laden (wie bisher)
		if (hud == null)
		{
			var hudScene = GD.Load<PackedScene>("res://UI/HUD/HUD.tscn");
			hud = hudScene.Instantiate<CanvasLayer>();
			AddChild(hud);
		}
		else
		{
			hud = GetNode<CanvasLayer>("HUD");
		}

		// PauseMenu attached to HUD!
		if (_pauseMenu == null && hud != null)
		{
			var pauseMenuScene = GD.Load<PackedScene>("res://Menu/PauseMenu/pauseMenu.tscn");
			_pauseMenu = pauseMenuScene.Instantiate<PauseMenu>();
			hud.AddChild(_pauseMenu);
			_pauseMenu.Visible = false;
		}
	}

	public override void _Process(double delta)
	{
		if (NetworkManager.Instance._isLocalHost) Server.Instance.syncHostWaveTimer();
		if (NetworkManager.Instance.SoloMode && _soloPlayer is not { alive: true })
			ShowGameOverScreen();

		if (!NetworkManager.Instance.SoloMode && !NetworkManager.Instance._isLocalHost)
		{
			var localPlayer = GetNodeOrNull<DefaultPlayer>($"Player_{Multiplayer.GetUniqueId()}");
			if (localPlayer != null && !localPlayer.alive)
				ShowGameOverScreen();
		}

		if (NetworkManager.Instance._isLocalHost || NetworkManager.Instance.SoloMode)
		{
			// Shop
			var currentWave = _globalWaveTimer.WaveCounter;
			if (currentWave > _lastLocalShopRound && currentWave < 5)
			{
				DebugIt("Check if shop should be started");
				_lastLocalShopRound = currentWave;
				if (_shopInstance == null)
				{
					DebugIt("Start new shop");
					_shopInstance = GD.Load<PackedScene>("res://UI/Shop/BossShop/bossShop.tscn").Instantiate();
					_shopInstance.Connect(nameof(BossShop.WeaponChosen), new Callable(this, nameof(OnWeaponChosen)));
					GetNodeOrNull<DefaultPlayer>("Player_1")
						.GetNodeOrNull<Camera2D>("Camera2D")
						.AddChild(_shopInstance);
				}
			}
		}

		// ScoreSystem
		foreach (var playerId in ScoreManager.PlayerScores.Keys)
		{
			ScoreManager.UpdateCombo(playerId, (float)delta);
		}
	}
	private void OnWeaponChosen(Weapon weaponType)
	{
		_selectedWeapon = weaponType.GetType().Name;
		_newWeaponPos++;

		var scene = _selectedWeapon switch
		{
			"Bow" => GD.Load<PackedScene>("res://Weapons/Ranged/Bow/bow.tscn"),
			"Crossbow" => GD.Load<PackedScene>("res://Weapons/Ranged/Crossbow/crossbow.tscn"),
			"FireStaff" => GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Firestaff/firestaff.tscn"),
			"Kunai" => GD.Load<PackedScene>("res://Weapons/Ranged/Kunai/kunai.tscn"),
			"Lightningstaff" => GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Lightningstaff/lightningstaff.tscn"),
			"Healstaff" => GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Healsftaff/healstaff.tscn"),
			"Dagger" => GD.Load<PackedScene>("res://Weapons/Melee/Dagger/dagger.tscn"),
			"Sword" => GD.Load<PackedScene>("res://Weapons/Melee/MasterSword/Sword.tscn"),
			"WarHammer" => GD.Load<PackedScene>("res://Weapons/Ranged/WarHammer/warHammer.tscn"),
			"DoubleBlade" => GD.Load<PackedScene>("res://Weapons/Melee/DoubleBlades/DoubleBlade.tscn"),
			_ => null
		};
		if (scene == null) return;

		var slot = GetChildren().OfType<DefaultPlayer>().FirstOrDefault()?.GetNode<Node2D>("WeaponSpawnPoints").GetChild<Node2D>(_newWeaponPos);
		var weapon = scene.Instantiate<Area2D>();

		if (NetworkManager.Instance._isLocalHost) // give to server -> send to clients
		{
			var id = weapon.GetInstanceId();
			weapon.Name = $"Weapon_{id}";
			weapon.SetMeta("OwnerId", 1);
			weapon.SetMeta("SlotIndex", _newWeaponPos);
			Server.Instance.Entities.Add((long)id, weapon);
		}

		slot?.AddChild(weapon);
		weapon.Position = Vector2.Up;

		_shopInstance.QueueFree();
		_shopInstance = null;
	}

	private void SpawnPlayer(long peerId)
	{
		// only solo mode cleanup
		if (NetworkManager.Instance.SoloMode)
			CleanupOldPlayers();

		var player = GD.Load<PackedScene>("res://Entities/Characters/Mage/mage.tscn").Instantiate<DefaultPlayer>();
		player.OwnerPeerId = peerId;
		player.Name = $"Player_{peerId}";


		PlayerCharacterData character = null;
		var characterId = 1;
		switch (NetworkManager.Instance.SoloMode)
		{
			case false when Server.Instance.PlayerSelections.TryGetValue(peerId, out character):
				characterId = character.CharacterId;
				break;
			case true:
				characterId = characterManager.LoadLastSelectedCharacterID();
				var health = characterManager.LoadHealthByID(characterId.ToString());
				var speed = characterManager.LoadSpeedByID(characterId.ToString());
				character = new PlayerCharacterData { CharacterId = characterId, Health = health, Speed = speed };
				break;
		}

		GD.Print("CharacterID: " + characterManager.LoadLastSelectedCharacterID());

		player = characterId switch
		{
			1 => GD.Load<PackedScene>("res://Entities/Characters/Archer/archer.tscn").Instantiate<DefaultPlayer>(),
			2 => GD.Load<PackedScene>("res://Entities/Characters/Assassin/assassin.tscn").Instantiate<DefaultPlayer>(),
			3 => GD.Load<PackedScene>("res://Entities/Characters/Knight/knight.tscn").Instantiate<DefaultPlayer>(),
			4 => GD.Load<PackedScene>("res://Entities/Characters/Mage/mage.tscn").Instantiate<DefaultPlayer>(),
			_ => player
		};
		player.OwnerPeerId = peerId;
		player.Name = $"Player_{peerId}";
		player.MaxHealth = character?.Health ?? 0;
		player.CurrentHealth = character?.Health ?? 0;
		player.Speed = character?.Speed ?? 0;
		DebugIt($"Spawned player {peerId} with characterId {characterId}, max health {player.MaxHealth}, current health {player.CurrentHealth}, speed {player.Speed}, health should be: {character.Health}");

		player.GlobalPosition = GetTree().GetNodesInGroup("PlayerSpawnPoints")
			.OfType<Node2D>()
			.ToList()
			.FindAll(spawnPoint => int.Parse(spawnPoint.Name) == _playerIndex)
			.FirstOrDefault()?.GlobalPosition ?? Vector2.Zero;

		// Add joystick to player
		var joystick = GD.Load<PackedScene>("res://UI/Joystick/joystick.tscn").Instantiate<Node2D>();
		if (peerId != 1)
		{
			joystick.Visible = false;
		}

		player.AddChild(joystick);
		player.Joystick = joystick;
		AddChild(player);

		if (!NetworkManager.Instance.SoloMode) Server.Instance.Entities[peerId] = player;

		// Connect health signal
		var healthNode = player.GetNodeOrNull<Health>("Health");
		if (healthNode != null)
		{
			healthNode.Connect(Health.SignalName.HealthDepleted, new Callable(this, nameof(OnPlayerDied)));
			healthNode.max_health = player.MaxHealth;
			healthNode.ResetHealth();
		}
		else
		{
			GD.PrintErr("Health node not found on player!");
		}

		_playerIndex++;
	}

	private void SendScoreToBackend(int score)
	{
		const string url = ServerConfig.BaseUrl + "/api/v1/protected/highscore/update";

		var headers = new[]
		{
			"Content-Type: application/json",
			"Authorization: Bearer " + SecureStorage.LoadToken()
		};

		var body = Json.Stringify(new Dictionary { { "score", score } });

		var err = _sendScoreRequest.Request(
			url,
			headers,
			HttpClient.Method.Post,
			body
		);
		if (err != Error.Ok)
			GD.PrintErr($"Score submit error: {err}");
	}

	private void OnScoreRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		DebugIt($"Score submit response: {responseCode}");
	}

	public void ShowGameOverScreen()
	{
		if (_gameOverScreen != null)
			return;

		var scene = GD.Load<PackedScene>("res://UI/GameOver/gameOverScreen.tscn");
		_gameOverScreen = scene.Instantiate<GameOverScreen>();
		AddChild(_gameOverScreen);

		long peerId = Multiplayer.GetUniqueId();
		var score = 0;
		if (ScoreManager.PlayerScores.TryGetValue(peerId, out var playerScore))
			score = playerScore;

		_gameOverScreen.SetScore(score);

		if (GameState.CurrentState == ConnectionState.Online)
			SendScoreToBackend(score);

	}

	public void OnPlayerDied(long peerId)
	{
		DebugIt($"Player {peerId} died! Switching camera to alive player or showing Game Over screen.");

		var score = 0;
		if (ScoreManager.PlayerScores.TryGetValue(peerId, out var playerScore))
			score = playerScore;

		// maybe we need for online multi
		/* 		if (_gameOverScreen == null)
					ShowGameOverScreen(); */

		//_gameOverScreen?.SetScore(score);

		// Remove player entity to prevent a leftover copy
		if (Server.Instance.Entities.TryGetValue(peerId, out var playerNode))
		{
			if (IsInstanceValid(playerNode))
			{
				playerNode.CallDeferred("queue_free");
				DebugIt($"Removed dead player node: Player_{peerId}");
			}
			DebugIt("Remove Player from server entities: " + peerId);
			Server.Instance.Entities.Remove(peerId);
		}

		// Find another player who is still alive
		var alivePlayers = GetTree()
			.GetNodesInGroup("player")
			.OfType<Node2D>()
			.Where(p => p.Name != $"Player_{peerId}")
			.ToList();
		DebugIt("Alive player count: " + (int)alivePlayers.Count);

		if (alivePlayers.Count > 0)
		{
			var target = alivePlayers[0];
			var cam = target.GetNodeOrNull<Camera2D>("Camera2D");
			if (cam == null) return;
			cam.MakeCurrent();
			DebugIt($"Camera switched to alive player: {target.Name}");
		}
		else
		{
			DebugIt("No alive players left.");
			ShowGameOverScreen();
			_gameOverScreen?.SetScore(score);
		}
	}



	private void DebugIt(string message)
	{
		if (_enableDebug) Debug.Print("GameRoot: " + message);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_cancel"))
		{
			var hud = GetNodeOrNull<CanvasLayer>("HUD");
			var pauseMenu = hud?.GetNodeOrNull<PauseMenu>("PauseMenu");
			if (pauseMenu != null && !pauseMenu.Visible)
			{
				pauseMenu.OpenPauseMenu();
			}
		}
	}

	private void CleanupOldPlayers()
	{
		foreach (var node in GetChildren().OfType<DefaultPlayer>().ToList())
		{
			node.QueueFree();
		}
	}

	public void CleanupAllLocal()
	{
		ScoreManager.PlayerScores.Clear();

		foreach (var node in GetChildren().OfType<DefaultPlayer>().ToList())
			node.QueueFree();

		foreach (var node in GetChildren().OfType<Node2D>().Where(n => n.Name.ToString().Contains("Joystick")).ToList())
			node.QueueFree();

		var hud = GetNodeOrNull<CanvasLayer>("HUD");
		if (hud != null)
			hud.QueueFree();

		if (_gameOverScreen != null)
		{
			_gameOverScreen.QueueFree();
			_gameOverScreen = null;
		}

		if (_pauseMenu != null)
		{
			_pauseMenu.QueueFree();
			_pauseMenu = null;
		}

		if (_shopInstance != null)
		{
			_shopInstance.QueueFree();
			_shopInstance = null;
		}
	}
}
