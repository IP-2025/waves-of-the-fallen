using Godot;
using System;

/// <summary>
/// GameRoot is the main entry point for the game. It handles:
/// - Loading the map
/// - Spawning the player
/// - Adding the wave timer
/// - Starting the enemy spawner
/// - Handling player death
/// </summary>
public partial class GameRoot : Node
{
    private Node2D _mainMap;

    public override void _Ready()
    {
        Engine.MaxFps = 60; // important! performance...

        // Initialize the game components
        SpawnMap("res://Scenes/Main.tscn");
        SpawnPlayer(GetTree().GetMultiplayer().GetUniqueId());
        AddWaveTimer("res://Scenes/Waves/WaveTimer.tscn");
        SpawnEnemySpawner("res://Scenes/Enemies/SpawnEnemies.tscn");

        // Start the game on the server
        NetworkManager.Instance.StartGame();
    }

    public override void _Process(double delta)
    {
        // Optional per-frame game logic
    }

    /// <summary>
    /// Loads and adds the main map to the scene.
    /// </summary>
    /// <param name="mapPath">Path to the map scene.</param>
    public void SpawnMap(string mapPath)
    {
        _mainMap = GD.Load<PackedScene>(mapPath).Instantiate<Node2D>();
        AddChild(_mainMap);
    }

    /// <summary>
    /// Spawns the player based on the selected character ID.
    /// </summary>
    /// <param name="peerId">The unique ID of the player.</param>
    public void SpawnPlayer(long peerId)
    {
        var characterManager = GetNode<CharacterManager>("/root/CharacterManager");
        int selectedCharacterId = characterManager.LoadLastSelectedCharacterID();

        string characterScenePath = selectedCharacterId switch
        {
            1 => "res://Scenes/Characters/archer.tscn",
            2 => "res://Scenes/Characters/assassin.tscn",
            3 => "res://Scenes/Characters/knight.tscn",
            4 => "res://Scenes/Characters/mage.tscn",
            _ => "res://Scenes/Characters/default_player.tscn"
        };

        var player = GD.Load<PackedScene>(characterScenePath).Instantiate<Node2D>();
        player.Name = $"Player_{peerId}";

        // Attach a joystick to the player
        var joystick = GD.Load<PackedScene>("res://Scenes/Joystick/joystick.tscn").Instantiate<Node2D>();
        player.AddChild(joystick);

        AddChild(player);
        GameManager.Instance.Entities[peerId] = player;

        // Connect the health depleted signal
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

    /// <summary>
    /// Adds the wave timer to the scene.
    /// </summary>
    /// <param name="waveTimerPath">Path to the wave timer scene.</param>
    public void AddWaveTimer(string waveTimerPath)
    {
        var waveTimer = GD.Load<PackedScene>(waveTimerPath).Instantiate<WaveTimer>();
        AddChild(waveTimer);
    }

    /// <summary>
    /// Spawns the enemy spawner in the scene.
    /// </summary>
    /// <param name="enemySpawnerPath">Path to the enemy spawner scene.</param>
    public void SpawnEnemySpawner(string enemySpawnerPath)
    {
        var spawner = GD.Load<PackedScene>(enemySpawnerPath).Instantiate<SpawnEnemies>();
        AddChild(spawner);
    }

    /// <summary>
    /// Handles the player's death and displays the Game Over screen.
    /// </summary>
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
        }
        else
        {
            GD.PrintErr("GameOver screen not found in main map!");
        }
    }
}
