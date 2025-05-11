using System;
using System.Diagnostics;
using Godot;

public partial class DefaultPlayer : CharacterBody2D
{
	[Export]
	public float Speed { get; set; }

	[Export]
	public int MaxHealth { get; set; }

	[Export]
	public int CurrentHealth { get; set; }
	public long OwnerPeerId { get; set; }

	public Node2D Joystick { get; set; }
	private Camera2D camera;
	private MultiplayerSynchronizer multiplayerSynchronizer;
	public bool enableDebug = false;
	
	public PackedScene BowScene = GD.Load<PackedScene>("res://Scenes/Weapons/bow.tscn");
	public PackedScene CrossbowScene = GD.Load<PackedScene>("res://Scenes/Weapons/crossbow.tscn");
	public PackedScene KunaiScene = GD.Load<PackedScene>("res://Scenes/Weapons/kunai.tscn");
	public PackedScene DaggerScene = GD.Load<PackedScene>("res://Scenes/Weapons/dagger.tscn");
	public PackedScene SwordScene = GD.Load<PackedScene>("res://Scenes/Weapons/Sword.tscn");
	public PackedScene SwordTest = GD.Load<PackedScene>("res://Scenes/Weapons/testsword.tscn");
	private int weaponsEquipped = 0;

	public override void _Ready()
	{
		GD.Print(SwordTest != null ? "Scene loaded!" : "Scene NOT found!");
		var playerClass = new Assassin(); // Instantiate Mage
		Speed = playerClass.Speed; // Override DefaultPlayer's Speed with Mage's Speed
		MaxHealth = playerClass.MaxHealth; // Override DefaultPlayer's MaxHealth with Mage's MaxHealth
		CurrentHealth = playerClass.CurrentHealth; // Set CurrentHealth to Mage's CurrentHealth
		GD.Print($"Assassin Speed applied: {Speed}, Assasin Health applied: {MaxHealth}");
		AddToGroup("player");
		CurrentHealth = MaxHealth;

		if (HasNode("Joystick"))
		{
			Joystick = GetNode<Node2D>("Joystick");
			var weaponSlot = GetNode<Node2D>("WeaponSpawnPoints").GetChild(weaponsEquipped) as Node2D;
			Area2D weapon = CreateWeaponForClass(playerClass);
			GD.Print($"Instantiated weapon: {weapon}");
			if (weapon != null)
			{
				weaponSlot.AddChild(weapon);
				weapon.Position = Vector2.Zero;
				// for multiplayer
				ulong id = weapon.GetInstanceId();
				weapon.Name = $"Weapon_{id}";
				weapon.SetMeta("OwnerId", OwnerPeerId );
				weapon.SetMeta("SlotIndex", weaponsEquipped);
				Server.Instance.Entities.Add((long)id, weapon);

				weaponsEquipped++;
				GD.Print($"Instantiated weapon: {weapon.Name}, at {weapon.GlobalPosition}");
			}
		}
		else
		{
			Joystick = null;
		}
	}

	private Area2D CreateWeaponForClass(object playerClass)
	{
		if (playerClass is Ranger)
			return BowScene.Instantiate() as Area2D;
		
		if (playerClass is Assassin)
			//GD.Print(daggerScene != null ? "OK" : "NOT FOUND");
			//return KunaiScene.Instantiate() as Area2D;
			return SwordTest.Instantiate() as Area2D;
			//return DaggerScene.Instantiate() as Area2D;
			//return SwordScene.Instantiate() as Area2D;
		// if (playerClass is Mage) return FireStaffScene.Instantiate() as Area2D;

		return null;
	}

	public override void _Process(double delta)
	{

	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Vector2.Zero;

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
		GD.Print("Default death behavior – no animation");
		QueueFree();
	}
}
