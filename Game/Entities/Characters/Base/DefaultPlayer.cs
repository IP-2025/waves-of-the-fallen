using System.Diagnostics;
using Game.Utilities.Backend;
using Game.Utilities.Multiplayer;
using Godot;
using System;

public partial class DefaultPlayer : CharacterBody2D
{


    [Export] public float Speed { get; set; }


    [Export] public int MaxHealth { get; set; }



    [Export] public int CurrentHealth { get; set; }


    [Export] public HttpRequest HttpRequest { get; set; }

    public long OwnerPeerId { get; set; }
    private int Gold { get; set; }

    [Export] protected NodePath animationPath;
    public Node2D Joystick { get; set; }
    private Camera2D _camera;
    private MultiplayerSynchronizer _multiplayerSynchronizer;
    private bool _enableDebug;

    public AnimationHandler animationHandler;
    public AnimatedSprite2D animation;

    private PackedScene _bowScene = GD.Load<PackedScene>("res://Weapons/Ranged/Bow/bow.tscn");
    private PackedScene _crossbowScene = GD.Load<PackedScene>("res://Weapons/Ranged/Crossbow/crossbow.tscn");
    private PackedScene _kunaiScene = GD.Load<PackedScene>("res://Weapons/Ranged/Kunai/kunai.tscn");

    private PackedScene _fireStaffScene =
        GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Firestaff/firestaff.tscn");

    private PackedScene _lightningStaffScene =
        GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Lightningstaff/lightningstaff.tscn");

    private PackedScene _daggerScene = GD.Load<PackedScene>("res://Weapons/Melee/Dagger/dagger.tscn");
    private PackedScene _swordScene = GD.Load<PackedScene>("res://Weapons/Melee/MasterSword/Sword.tscn");
    public PackedScene _medicineBagScene = GD.Load<PackedScene>("res://Weapons/Utility/MedicineBag/medicineBag.tscn");
    private PackedScene _healStaffScene = GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Healsftaff/healstaff.tscn");
    private int _weaponsEquipped;

    private WaveTimer _waveTimer;
    protected CharacterManager CharacterManager;
    private bool _requestSent;
    private bool _alreadyDead;
    private Vector2 lastPos = Vector2.Zero;

    public override void _Ready()
    {
        _alreadyDead = false;
        _requestSent = false;
        AddToGroup("player");
        /* 		base._Ready(); */

        CharacterManager = GetNode<CharacterManager>("/root/CharacterManager");
        var selectedCharacterId = CharacterManager.LoadLastSelectedCharacterID();

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
        if (animationPath != null && !animationPath.IsEmpty)
        {
            animation = GetNode<AnimatedSprite2D>(animationPath);
            animationHandler = new AnimationHandler(animation);
        }
        else
        {
            GD.PushError($"{Name} has no animationPath set!");
        }
    }

    private Area2D CreateWeaponForClass(object playerClass)
    {
        return playerClass switch
        {
            Archer => _bowScene.Instantiate() as Area2D,
            // return _medicineBagScene.Instantiate() as Area2D,
            Assassin => _kunaiScene.Instantiate() as Area2D,
            //return _healStaffScene.Instantiate() as Area2D;
            Mage => _fireStaffScene.Instantiate() as Area2D,
            //return DaggerScene.Instantiate() as Area2D;
            Knight => _swordScene.Instantiate() as Area2D,
            _ => null
        };
    }

    public override void _Process(double delta)
    {
        Vector2 deltaPos = GlobalPosition - lastPos;
        if (_waveTimer != null) return;
        var cam = GetNodeOrNull<Camera2D>("Camera2D");
        if (cam == null) return;
        _waveTimer = cam.GetNodeOrNull<WaveTimer>("WaveTimer");
        if (_waveTimer != null)
        {
            _waveTimer.WaveEnded += OnWaveTimerTimeout;
        }
        // Flip based on movement direction
        if (animation != null && Math.Abs(deltaPos.X) > 1e-2)
            animation.FlipH = deltaPos.X < 0;

        // Walk/Idle Animation based on movement
        if (animationHandler != null)
        {
            if (deltaPos.Length() > 1e-2)
                animationHandler.UpdateAnimationState(false, deltaPos);
            else
                animationHandler.UpdateAnimationState(false, Vector2.Zero);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        lastPos = GlobalPosition;

        if (animationHandler != null && animationHandler.IsDying)
        {
            Velocity = Vector2.Zero;
            MoveAndSlide();
            return;
        }

        var direction = Vector2.Zero;

        // Get joystick directly as child
        var joystick = GetNodeOrNull<Joystick>("Joystick");
        if (joystick != null && joystick.PosVector != Vector2.Zero)
        {
            direction = joystick.PosVector;
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
        if (_alreadyDead) return;

        _alreadyDead = true;
        Velocity = Vector2.Zero;
        SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("playerDies"),
            GlobalPosition);
        animationHandler?.SetDeath();

        GD.Print("Player died with this amount of gold: " + Gold);
        CharacterManager.AddGold(Gold);

        if (GameState.CurrentState == ConnectionState.Offline)
        {
            QueueFree();
        }
        else if (!_requestSent)
        {
            _requestSent = true;
            var token = SecureStorage.LoadToken();
            if (string.IsNullOrEmpty(token)) return;
            const string url = $"{ServerConfig.BaseUrl}/api/v1/protected/gold/add";
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

    public void OnHit()
    {
        animationHandler?.SetHit();
    }
}