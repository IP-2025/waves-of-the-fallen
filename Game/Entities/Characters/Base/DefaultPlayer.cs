using System.Diagnostics;
using Game.Utilities.Backend;
using Game.Utilities.Multiplayer;
using Godot;

public partial class DefaultPlayer : CharacterBody2D
{
    [Export] public float Speed { get; set; }

    [Export] public int MaxHealth { get; set; }

    [Export] public int CurrentHealth { get; set; }

    [Export] public HttpRequest HttpRequest { get; set; }
    public long OwnerPeerId { get; set; }
    private int Gold { get; set; }

    public Node2D Joystick { get; set; }
    private Camera2D _camera;
    private MultiplayerSynchronizer _multiplayerSynchronizer;
    private bool _enableDebug;

    private PackedScene _bowScene = GD.Load<PackedScene>("res://Weapons/Ranged/Bow/bow.tscn");
    private PackedScene _crossbowScene = GD.Load<PackedScene>("res://Weapons/Ranged/Crossbow/crossbow.tscn");
    private PackedScene _kunaiScene = GD.Load<PackedScene>("res://Weapons/Ranged/Kunai/kunai.tscn");

    private PackedScene _fireStaffScene =
        GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Firestaff/firestaff.tscn");

    private PackedScene _lightningStaffScene =
        GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Lightningstaff/lightningstaff.tscn");

    private PackedScene _daggerScene = GD.Load<PackedScene>("res://Weapons/Melee/Dagger/dagger.tscn");
    private PackedScene _swordScene = GD.Load<PackedScene>("res://Weapons/Melee/MasterSword/Sword.tscn");
    private int _weaponsEquipped;

    private WaveTimer _waveTimer;
    private CharacterManager _characterManager;
    private bool _requestSent;

    public override void _Ready()
    {
        AddToGroup("player");

        _characterManager = GetNode<CharacterManager>("/root/CharacterManager");
        var selectedCharacterId = _characterManager.LoadLastSelectedCharacterID();

        HttpRequest.Connect("request_completed", new Callable(this, nameof(OnRequestCompleted)));

        object playerClass = selectedCharacterId switch
        {
            1 => new Archer(),
            2 => new Assassin(),
            3 => new Knight(),
            4 => new Mage(),
            _ => new DefaultPlayer()
        };

        // Equip weapon for the selected class
        var weaponSlot = GetNode<Node2D>("WeaponSpawnPoints").GetChild(_weaponsEquipped) as Node2D;
        var weapon = CreateWeaponForClass(playerClass);

        if (weapon == null) return;
        weaponSlot?.AddChild(weapon);
        weapon.Position = Vector2.Zero;

        // for multiplayer
        var id = weapon.GetInstanceId();
        weapon.Name = $"Weapon_{id}";
        weapon.SetMeta("OwnerId", OwnerPeerId);
        weapon.SetMeta("SlotIndex", _weaponsEquipped);
        Server.Instance.Entities.Add((long)id, weapon);

        _weaponsEquipped++;
    }

    private Area2D CreateWeaponForClass(object playerClass)
    {
        return playerClass switch
        {
            Archer => _bowScene.Instantiate() as Area2D,
            Assassin => _kunaiScene.Instantiate() as Area2D,
            //return LightningStaffScene.Instantiate() as Area2D;
            Mage => _fireStaffScene.Instantiate() as Area2D,
            //return DaggerScene.Instantiate() as Area2D;
            Knight => _swordScene.Instantiate() as Area2D,
            _ => null
        };
    }

    public override void _Process(double delta)
    {
        if (_waveTimer != null) return;
        var cam = GetNodeOrNull<Camera2D>("Camera2D");
        if (cam == null) return;
        _waveTimer = cam.GetNodeOrNull<WaveTimer>("WaveTimer");
        if (_waveTimer != null)
        {
            _waveTimer.WaveEnded += OnWaveTimerTimeout;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        var direction = Vector2.Zero;

        // Check if joystick exists and if it's being used
        if (Joystick != null)
        {
            var joystickDirection = (Vector2)Joystick.Get("PosVector");
            if (joystickDirection != Vector2.Zero)
            {
                direction = joystickDirection;
            }
        }

        // If no joystick input, fallback to keyboard
        if (direction == Vector2.Zero)
        {
            direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        }

        Velocity = direction * Speed;
        MoveAndSlide();
    }

    protected virtual void UseAbility()
    {
        GD.Print("Ability placeholder for all classes");
    }

    private void DebugIt(string message)
    {
        if (_enableDebug)
        {
            Debug.Print(message);
        }
    }

    public void Die()
    {
        SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("playerDies"),
            GlobalPosition);

        GD.Print("Player died with this amount of gold: " + Gold);
        _characterManager.AddGold(Gold);

        if (GameState.CurrentState == ConnectionState.Offline)
        {
            QueueFree();
        }
        else if (!_requestSent)
        {
            _requestSent = true;
            var token = SecureStorage.LoadToken();
            if (string.IsNullOrEmpty(token)) return;
            const string url = $"{ServerConfig.BaseUrl}/api/v1/protected/addGold";
            var headers = new[] { $"Authorization: Bearer {token}", "Content-Type: application/json" };
            var body = Json.Stringify(new Godot.Collections.Dictionary
            {
                { "gold", Gold }
            });
            var err = HttpRequest.Request(
                url,
                headers,
                HttpClient.Method.Post,
                body
            );
            if (err != Error.Ok)
                GD.PrintErr($"AuthRequest error: {err}");
        }
    }

    private void OnWaveTimerTimeout()
    {
        GD.Print("OnWaveTimerTimeout");
        Gold += 10;

        GD.Print("Gold: " + Gold);
    }

    private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        if (responseCode == 200)
        {
            GD.Print("Request completed successfully.");
        }
        else
        {
            GD.PrintErr($"Request failed with response code: {responseCode}");
        }

        QueueFree();
    }
}