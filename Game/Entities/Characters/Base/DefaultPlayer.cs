using System;
using System.Diagnostics;
using Game.Utilities.Backend;
using Game.Utilities.Multiplayer;
using Godot;

public partial class DefaultPlayer : CharacterBody2D
{


	[Export] public float Speed { get; set; }


	[Export] public int MaxHealth { get; set; }

	[Export] public int Strength { get; set; }
	[Export] public int Dexterity { get; set; }
	[Export] public int Intelligence { get; set; }

	[Export] public int CurrentHealth { get; set; }


	[Export] public HttpRequest HttpRequest { get; set; }

	public long OwnerPeerId { get; set; }
	private int Gold { get; set; }

	[Export] protected NodePath animationPath;
	public Node2D Joystick { get; set; }
	public Node2D Ability { get; set; }
	private Camera2D _camera;
	private MultiplayerSynchronizer _multiplayerSynchronizer;
	private bool _enableDebug;

	public AnimationHandler animationHandler;
	public AnimatedSprite2D animation;
	public bool alive = false;
	private PackedScene _bowScene = GD.Load<PackedScene>("res://Weapons/Ranged/Bow/bow.tscn");
	private PackedScene _crossbowScene = GD.Load<PackedScene>("res://Weapons/Ranged/Crossbow/crossbow.tscn");
	private PackedScene _kunaiScene = GD.Load<PackedScene>("res://Weapons/Ranged/Kunai/kunai.tscn");

	private PackedScene _fireStaffScene = GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Firestaff/firestaff.tscn");

	private PackedScene _lightningStaffScene = GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Lightningstaff/lightningstaff.tscn");

	private PackedScene _daggerScene = GD.Load<PackedScene>("res://Weapons/Melee/Dagger/dagger.tscn");
	private PackedScene _swordScene = GD.Load<PackedScene>("res://Weapons/Melee/MasterSword/Sword.tscn");
	private PackedScene _medicineBagScene = GD.Load<PackedScene>("res://Weapons/Utility/MedicineBag/medicineBag.tscn");
	private PackedScene _healStaffScene = GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Healsftaff/healstaff.tscn");
	private PackedScene _doubleBladeScene = GD.Load<PackedScene>("res://Weapons/Melee/DoubleBlades/DoubleBlade.tscn");
	private PackedScene _warHammerScene = GD.Load<PackedScene>("res://Weapons/Ranged/WarHammer/warHammer.tscn");
	private int _weaponsEquipped;
	public PackedScene _boostStrScene = GD.Load<PackedScene>("res://UI/Ability/Ablities/boost_strength.tscn");
	public PackedScene _boostDexScene = GD.Load<PackedScene>("res://UI/Ability/Ablities/boost_dexterity.tscn");
	public PackedScene _boostIntScene = GD.Load<PackedScene>("res://UI/Ability/Ablities/boost_intelligence.tscn");
	public PackedScene _dashScene = GD.Load<PackedScene>("res://UI/Ability/Ablities/speed_up.tscn");
	public PackedScene _shieldScene = GD.Load<PackedScene>("res://UI/Ability/Ablities/shield.tscn");
	public PackedScene _abilityScene;
	public int _abilityIndex = 4; //Hier ändern noch, speichern in savedata oben export variable
	private WaveTimer _waveTimer;
	protected CharacterManager CharacterManager;
	private bool _requestSent;
	private bool _alreadyDead;
	private Vector2 _lastPos = Vector2.Zero;

	public override void _Ready()
	{
		alive = true;
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

		_abilityScene = GetAbilityForPlayer(selectedCharacterId);

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
			//Assassin => _kunaiScene.Instantiate() as Area2D,
			//Assassin => _doubleBladeScene.Instantiate() as Area2D,
			Assassin => _daggerScene.Instantiate() as Area2D,
			//return _healStaffScene.Instantiate() as Area2D;
			Mage => _fireStaffScene.Instantiate() as Area2D,
			Knight => _swordScene.Instantiate() as Area2D,
			//Knight => _warHammerScene.Instantiate() as Area2D,
			_ => null
		};
	}

	private PackedScene GetAbilityForPlayer(int id)
	{
		_abilityIndex = id * 10 + CharacterManager.LoadAbilityChosenByID(id.ToString());

		return _abilityIndex switch
		{
			11 => _boostDexScene,
			//12 => _
			21 => _dashScene,
			//22 => _
			31 => _boostStrScene,
			32 => _shieldScene,
			41 => _boostIntScene,
			//42 => _
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
		if (direction == Vector2.Zero && OwnerPeerId == Multiplayer.GetUniqueId()) //w a s d only for own player
		{
			direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		}

		Velocity = direction * Speed;
		MoveAndSlide();

		// Check if in solo (offline) mode or multiplayer (clients cant move, server handles it)
		if (NetworkManager.Instance._soloMode || NetworkManager.Instance._isServer) UpdateTransform(Velocity);
	}

	public virtual void UseAbility()
	{
		AddChild(_abilityScene.Instantiate());
	}

	public PackedScene GetAbilityScene()
	{
		return _abilityScene;
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

		alive = false;
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

	private void UpdateTransform(Vector2 velocity)
	{
		if (animation == null) return;
		// Flip based on movement direction
		animation.FlipH = velocity.X > 0 ? false : true;
		// Walk/Idle Animation based on movement
		animationHandler.UpdateAnimationState(false, velocity);
	}

	public void OnHit()
	{
		animationHandler?.SetHit();
	}
}
