using Godot;
using System;
using System.Diagnostics;
using System.Linq;
using Game.Utilities.Multiplayer;

// GameRoot is the main entry point for the game. It is responsible for loading the map, spawning the player, starting the enemy spawner and so on.
public partial class GameRoot : Node
{
    private Node2D _mainMap;
    private int _playerIndex = 0; // player index for spawning players
    private bool _isServer = false;
    private bool _enableDebug = false;
    private WaveTimer _globalWaveTimer;
    public bool _soloMode = false;
    public int _soloSelectedCharacterId = 1;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Engine.MaxFps = 60; // important! performance...

        if (!_soloMode)
            _isServer = GetTree().GetMultiplayer().IsServer();

        // Load map and store reference
        SpawnMap("res://Maps/GrassMap/Main.tscn");

        // Instantiate one global WaveTimer for server-wide access
        if (!_isServer && !_soloMode) return;

        var waveTimerScene = GD.Load<PackedScene>("res://Utilities/Gameflow/Waves/WaveTimer.tscn");
        _globalWaveTimer = waveTimerScene.Instantiate<WaveTimer>();
        _globalWaveTimer.Name = "GlobalWaveTimer";
        if (_isServer)
            AddChild(_globalWaveTimer);

        if (_soloMode)
        {
            SpawnPlayer(1);
            GetChildren().OfType<DefaultPlayer>().FirstOrDefault().GetNodeOrNull<Camera2D>("Camera2D").AddChild(_globalWaveTimer);
        }
        else // its the server
        {
            // Server spawns all players
            foreach (var peerId in GetTree().GetMultiplayer().GetPeers())
            {
                DebugIt($"Server spawning player {peerId}");
                SpawnPlayer(peerId);
            }
        }

        // Start enemy spawner
        SpawnEnemySpawner("res://Utilities/Gameflow/Spawn/SpawnEnemies.tscn");
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
        if (!_soloMode)
        {
            characterId = Server.Instance.PlayerSelections[peerId];
        }

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

        if (!_soloMode)
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

        _playerIndex++;
    }

    private void SpawnEnemySpawner(string enemySpawnerPath)
    {
        var spawner = GD.Load<PackedScene>(enemySpawnerPath).Instantiate<SpawnEnemies>();
        AddChild(spawner);
    }

    public void OnPlayerDied()
    {
        DebugIt("Player died! Switching camera to alive player.");
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

        if (_soloMode) return;

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