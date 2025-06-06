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
	private int _playerIndex = 0; // player index for spawning players
	private bool _isServer = false;
	private bool _enableDebug = false;
	private WaveTimer _globalWaveTimer;
	private int _soloSelectedCharacterId = 1;
	private DefaultPlayer _soloPlayer;

	// Shop dirty workaround
	private int _lastLocalShopRound = 1;
	private Node _shopInstance;
	private int _newWeaponPos = 0;
	string _selectedWeapon = "";

	// GameOver
	private GameOverScreen _gameOverScreen;
	public bool _soloMode = false;
	public int _soloSelectedCharacterId = 1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Engine.MaxFps = 60; // important! performance...

		// get selected character
		var characterManager = GetNode<CharacterManager>("/root/CharacterManager");
		_soloSelectedCharacterId = characterManager.LoadLastSelectedCharacterID();

		if (!NetworkManager.Instance._soloMode) _isServer = GetTree().GetMultiplayer().IsServer();

		// Load map and store reference
		SpawnMap("res://Maps/GrassMap/Main.tscn");

		if (!_isServer && !NetworkManager.Instance._soloMode) return; // clients return

		// Instantiate one global WaveTimer for server-wide access
		var waveTimerScene = GD.Load<PackedScene>("res://Utilities/Gameflow/Waves/WaveTimer.tscn");
		_globalWaveTimer = waveTimerScene.Instantiate<WaveTimer>();
		if (_isServer)
		{
			_globalWaveTimer.Name = "GlobalWaveTimer";
			AddChild(_globalWaveTimer);
			// Server spawns all players
			foreach (var peerId in GetTree().GetMultiplayer().GetPeers())
			{
				DebugIt($"Server spawning player {peerId}");
				SpawnPlayer(peerId);
			}
		}

		if (NetworkManager.Instance._soloMode)
		{
			SpawnPlayer(1);
			_soloPlayer = GetNodeOrNull<DefaultPlayer>("Player_1");
			GetChildren().OfType<DefaultPlayer>().FirstOrDefault().GetNodeOrNull<Camera2D>("Camera2D").AddChild(_globalWaveTimer);
		}

		// Start enemy spawner
		SpawnEnemySpawner("res://Utilities/Gameflow/Spawn/SpawnEnemies.tscn");
	}

	public override void _Process(double delta)
	{
		// kind of a silly workaround, however its better than re implementing the shop system...
		if (!NetworkManager.Instance._soloMode) return;

		if (_soloPlayer == null || !_soloPlayer.alive) ShowGameOverScreen();

		int currentWave = _globalWaveTimer.WaveCounter;
		if (currentWave > _lastLocalShopRound && currentWave < 5)
		{
			_lastLocalShopRound = currentWave;
			if (_shopInstance == null)
			{
				_shopInstance = GD.Load<PackedScene>("res://UI/Shop/BossShop/bossShop.tscn").Instantiate();
				_shopInstance.Connect(nameof(BossShop.WeaponChosen), new Callable(this, nameof(OnWeaponChosen)));
				_soloPlayer.GetNodeOrNull<Camera2D>("Camera2D").AddChild(_shopInstance);
			}
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

		var slot = GetChildren().OfType<DefaultPlayer>().FirstOrDefault().GetNode<Node2D>("WeaponSpawnPoints").GetChild<Node2D>(_newWeaponPos);
		var weapon = scene.Instantiate<Area2D>();
		slot.AddChild(weapon);
		weapon.Position = Vector2.Up;

		_shopInstance.QueueFree();
		_shopInstance = null;
	}

	private void SpawnMap(string mapPath)
	{
		_mainMap = GD.Load<PackedScene>(mapPath).Instantiate<Node2D>();
		AddChild(_mainMap);
	}

	private void SpawnPlayer(long peerId)
	{
		var player = GD.Load<PackedScene>("res://Entities/Characters/Mage/mage.tscn").Instantiate<DefaultPlayer>();
		player.OwnerPeerId = peerId;
		player.Name = $"Player_{peerId}";


		int characterId = _soloSelectedCharacterId;
		if (!NetworkManager.Instance._soloMode) characterId = Server.Instance.PlayerSelections[peerId];
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


		// Get spawn point from PlayerSpawnPoints group
		player.GlobalPosition = GetTree().GetNodesInGroup("PlayerSpawnPoints")
			.OfType<Node2D>()
			.ToList()
			.FindAll(spawnPoint => int.Parse(spawnPoint.Name) == _playerIndex)
			.FirstOrDefault()?.GlobalPosition ?? Vector2.Zero;

		// Add joystick to player
		var joystick = GD.Load<PackedScene>("res://UI/Joystick/joystick.tscn").Instantiate<Node2D>();
		player.AddChild(joystick);
		player.Joystick = joystick;
		// var waveTimer = GD.Load<PackedScene>("res://Utilities/Gameflow/Waves/WaveTimer.tscn").Instantiate<WaveTimer>();
		// player.GetNode<Camera2D>("Camera2D").AddChild(_globalWaveTimer);
		AddChild(player);

		if (!NetworkManager.Instance._soloMode) Server.Instance.Entities[peerId] = player;

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

		_playerIndex++;
	}

	private void SpawnEnemySpawner(string enemySpawnerPath)
	{
		var spawner = GD.Load<PackedScene>(enemySpawnerPath).Instantiate<SpawnEnemies>();
		AddChild(spawner);
	}

	public void ShowGameOverScreen()
	{
		if (_gameOverScreen != null)
		{
			return;
		}
		var scene = GD.Load<PackedScene>("res://UI/GameOver/gameOverScreen.tscn");
		_gameOverScreen = scene.Instantiate<GameOverScreen>();
		AddChild(_gameOverScreen);
		_gameOverScreen.SetScore(0); //Show Score 0 for now, you can set it later

	}

	public void OnPlayerDied()
	{
		DebugIt("Player died! Switching camera to alive player.");
		DebugIt("Player died! Showing Game Over screen.");

		long peerId = Multiplayer.GetUniqueId();

		// Remove player entity to prevent a leftover copy
		if (Server.Instance.Entities.TryGetValue(peerId, out var playerNode))
		{
			if (IsInstanceValid(playerNode))
			{
				playerNode.QueueFree();
				DebugIt($"Removed dead player node: Player_{peerId}");
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
				DebugIt($"Camera switched to alive player: {target.Name}");
			}
		}
		else
		{
			DebugIt("No alive players left.");
		}
	}



	private void DebugIt(string message)
	{
		if (_enableDebug) Debug.Print("GameRoot: " + message);
	}
}
