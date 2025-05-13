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
	
	public Node2D weaponSlot { get; set; }
	public Node2D Joystick { get; set; }
	private Camera2D camera;
	private MultiplayerSynchronizer multiplayerSynchronizer;
	public bool enableDebug = false;
	
	public PackedScene BowScene = GD.Load<PackedScene>("res://Scenes/Weapons/bow.tscn");
	public PackedScene CrossbowScene = GD.Load<PackedScene>("res://Scenes/Weapons/crossbow.tscn");
	public PackedScene KunaiScene = GD.Load<PackedScene>("res://Scenes/Weapons/kunai.tscn");
	public PackedScene DaggerScene = GD.Load<PackedScene>("res://Scenes/Weapons/dagger.tscn");
	public PackedScene SwordScene = GD.Load<PackedScene>("res://Scenes/Weapons/Sword.tscn");
	private int weaponsEquipped = 0;

	public override void _Ready()
	{
		AddToGroup("player");
/* 		base._Ready(); */

		var characterManager = GetNode<CharacterManager>("/root/CharacterManager");
		int selectedCharacterId = characterManager.LoadLastSelectedCharacterID();
 
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
 		var weaponSlot = GetNode<Node2D>("WeaponSpawnPoints").GetChild(weaponsEquipped) as Node2D;
		Area2D weapon = CreateWeaponForClass(playerClass);

		if (weapon != null)
		{
			weaponSlot.AddChild(weapon);
			weapon.Position = Vector2.Zero;

			// for multiplayer
			ulong id = weapon.GetInstanceId();
			weapon.Name = $"Weapon_{id}";
			weapon.SetMeta("OwnerId", OwnerPeerId);
			weapon.SetMeta("SlotIndex", weaponsEquipped);
			Server.Instance.Entities.Add((long)id, weapon);

			weaponsEquipped++;
		} 
	}

	private Area2D CreateWeaponForClass(object playerClass)
	{
		if (playerClass is Archer)
			return BowScene.Instantiate() as Area2D;
		
		if (playerClass is Assassin)
			//GD.Print(daggerScene != null ? "OK" : "NOT FOUND");
			//return KunaiScene.Instantiate() as Area2D;
			return KunaiScene.Instantiate() as Area2D;
		if (playerClass is Warrior)
			//return DaggerScene.Instantiate() as Area2D;
			return SwordScene.Instantiate() as Area2D;
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
