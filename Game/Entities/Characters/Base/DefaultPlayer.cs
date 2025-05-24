using System.Diagnostics;
using System.Threading;
using Godot;
using Game.Utilities.Multiplayer;

public partial class DefaultPlayer : CharacterBody2D
{
    [Export] public float Speed { get; set; }

    [Export] public int MaxHealth { get; set; }

    [Export] public int CurrentHealth { get; set; }
    public long OwnerPeerId { get; set; }
    private int Coins { get; set; } = 0;

    public Node2D Joystick { get; set; }
    private Camera2D _camera;
    private MultiplayerSynchronizer _multiplayerSynchronizer;
    private bool _enableDebug = false;

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

    public override void _Ready()
    {
        AddToGroup("player");

        var characterManager = GetNode<CharacterManager>("/root/CharacterManager");
        var selectedCharacterId = characterManager.LoadLastSelectedCharacterID();
        
        // TODO: Get the timer from the scene tree
        
/* 		GetNodeOrNull<Node2D>("Archer")?.Hide();
        GetNodeOrNull<Node2D>("Assassin")?.Hide();
        GetNodeOrNull<Node2D>("Knight")?.Hide();
        GetNodeOrNull<Node2D>("Mage")?.Hide();
        

        // Zeige nur den ausgewählten Charakter
        string selectedClassNodeName = selectedCharacterId switch
        {
            1 => "Archer",
            2 => "Assassin",
            3 => "Knight",
            4 => "Mage",
            _ => "Archer" // Standardwert
        }; */

        /* 		var selectedClassNode = GetNodeOrNull<Node2D>(selectedClassNodeName);
               if (selectedClassNode != null)
               {
                   selectedClassNode.Show();
                   GD.Print($"Selected class: {selectedClassNodeName}");
               }
               else
               {
                   GD.PrintErr($"Class node '{selectedClassNodeName}' not found!");
               }*/

        object playerClass = selectedCharacterId switch
        {
            1 => new Archer(),
            2 => new Assassin(),
            3 => new Knight(),
            4 => new Mage(),
            _ => new DefaultPlayer()
        };

/* 		GD.Print($"Selected Character: {playerClass.GetType().Name}"); */

        // Set attributes based on the selected class

        //------------------
/* 		if (playerClass is DefaultPlayer defaultPlayer)
        {
            Speed = defaultPlayer.Speed;
            MaxHealth = defaultPlayer.MaxHealth;
            CurrentHealth = defaultPlayer.MaxHealth;
        }
 */
/* 		AddToGroup("player");
        CurrentHealth = MaxHealth;

        OwnerPeerId = Multiplayer.GetUniqueId();
        GD.Print($"OwnerPeerId set to: {OwnerPeerId}");

        // Synchronize MaxHealth with the Health node
        var healthNode = GetNodeOrNull<Health>("Health");
        if (healthNode != null)
        {
            healthNode.max_health = MaxHealth;
            healthNode.ResetHealth(); // Reset health to max_health
        }
        else
        {
            GD.PrintErr("Health node not found!");
        } */

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
        if (_waveTimer == null)
        {
            var cam = GetNodeOrNull<Camera2D>("Camera2D");
            if (cam != null)
            {
                _waveTimer = cam.GetNodeOrNull<WaveTimer>("WaveTimer");
                if (_waveTimer != null)
                {
                    _waveTimer.WaveEnded += OnWaveTimerTimeout;
                }
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        var direction = Vector2.Zero;

        // Check if joystick exists and if it's being used
        if (Joystick != null)
        {
            Vector2 joystickDirection = (Vector2)Joystick.Get("PosVector");
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

        // // Play Animations
        // if (animationPlayer != null)
        // {
        // 	if (direction != Vector2.Zero)
        // 	{
        // 		if (!animationPlayer.IsPlaying() || animationPlayer.CurrentAnimation != "walk")
        // 			animationPlayer.Play("walk");
        // 	}
        // 	else
        // 	{
        // 		if (!animationPlayer.IsPlaying() || animationPlayer.CurrentAnimation != "idle")
        //
        // 			animationPlayer.Play("idle");
        //
        //
        // 	}
        // }
        //
        // if (archerSprite != null && direction != Vector2.Zero)
        // {
        // 	archerSprite.FlipH = direction.X < 0;
        // }
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

    public virtual void Die()
    {
        GD.Print("Player dies with Coins: " + Coins);
        SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("playerDies"), GlobalPosition);
        QueueFree();
    }
    
    private void OnWaveTimerTimeout()
    {
        Coins += 10; 
    }
}