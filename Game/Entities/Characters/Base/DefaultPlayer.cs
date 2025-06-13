using System;
using System.Diagnostics;
using Godot;
using Game.Utilities.Multiplayer;

public partial class DefaultPlayer : CharacterBody2D
{
	[Export]
	public float Speed { get; set; }

	[Export]
	public int MaxHealth { get; set; }

	[Export]
	public int CurrentHealth { get; set; }
	public long OwnerPeerId { get; set; }

	[Export] protected NodePath animationPath;

	public Node2D Joystick { get; set; }
	private Camera2D camera;
	private MultiplayerSynchronizer multiplayerSynchronizer;
	public bool enableDebug = false;

	public AnimationHandler animationHandler;
	public AnimatedSprite2D animation;
	
	public PackedScene BowScene = GD.Load<PackedScene>("res://Weapons/Ranged/Bow/bow.tscn");
	public PackedScene CrossbowScene = GD.Load<PackedScene>("res://Weapons/Ranged/Crossbow/crossbow.tscn");
	public PackedScene KunaiScene = GD.Load<PackedScene>("res://Weapons/Ranged/Kunai/kunai.tscn");
	public PackedScene FireStaffScene = GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Firestaff/firestaff.tscn");
	public PackedScene LightningStaffScene = GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Lightningstaff/lightningstaff.tscn");
	public PackedScene DaggerScene = GD.Load<PackedScene>("res://Weapons/Melee/Dagger/dagger.tscn");
	public PackedScene SwordScene = GD.Load<PackedScene>("res://Weapons/Melee/MasterSword/Sword.tscn");
	private int weaponsEquipped = 0;

	public override void _Ready()
	{
		AddToGroup("player");

		var characterManager = GetNode<CharacterManager>("/root/CharacterManager");
		int selectedCharacterId = characterManager.LoadLastSelectedCharacterID();

		object playerClass = selectedCharacterId switch
		{
			1 => new Archer(),
			2 => new Assassin(),
			3 => new Knight(),
			4 => new Mage(),
			_ => new DefaultPlayer()
		};

		var healthNode = GetNodeOrNull<Health>("Health");
		if (healthNode != null)
		{
			healthNode.max_health = MaxHealth;
			healthNode.ResetHealth();
		}
		else
		{
			GD.PrintErr("Health node not found!");
		} 

		var weaponSlot = GetNode<Node2D>("WeaponSpawnPoints").GetChild(weaponsEquipped) as Node2D;
		Area2D weapon = CreateWeaponForClass(playerClass);

		if (weapon != null)
		{
			weaponSlot.AddChild(weapon);
			weapon.Position = Vector2.Zero;

			ulong id = weapon.GetInstanceId();
			weapon.Name = $"Weapon_{id}";
			weapon.SetMeta("OwnerId", OwnerPeerId);
			weapon.SetMeta("SlotIndex", weaponsEquipped);
			Server.Instance.Entities.Add((long)id, weapon);

			weaponsEquipped++;
		} 

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
		if (playerClass is Archer)
			return BowScene.Instantiate() as Area2D;
		
		if (playerClass is Assassin)
			return KunaiScene.Instantiate() as Area2D;

		if (playerClass is Mage) 
			return FireStaffScene.Instantiate() as Area2D;
		if (playerClass is Knight)
			return SwordScene.Instantiate() as Area2D;
		
		return null;
	}

	public override void _Process(double delta) {}

	public override void _PhysicsProcess(double delta)
	{
		if (animationHandler != null && animationHandler.IsDying)
		{
			Velocity = Vector2.Zero;
			MoveAndSlide();
			return;
		}

		Vector2 direction = Vector2.Zero;

		var joystick = GetNodeOrNull<Joystick>("Joystick");
		if (joystick != null && joystick.PosVector != Vector2.Zero)
		{
			direction = joystick.PosVector;
		}

		if (direction == Vector2.Zero)
		{
			direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		}
		
		Velocity = direction * Speed;
		MoveAndSlide();
	}

	public virtual void UseAbility()
	{
		GD.Print("Ability placeholder for all classes");
	}

	private void DebugIt(string message)
	{
		if (enableDebug)
		{
			Debug.Print(message);
		}
	}

	public virtual void Die()
	{
		GD.Print($"[DefaultPlayer.Die] Called on: {Name}");

		Velocity = Vector2.Zero;
		SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("playerDies"), GlobalPosition);
		animationHandler?.SetDeath();
		QueueFree();
		
		GD.Print("DefaultPlayer.Die() called");
	}
	
	public void OnHit()
	{
		animationHandler?.SetHit();
	}
}
