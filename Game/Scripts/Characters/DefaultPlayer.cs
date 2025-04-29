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

	public Node2D Joystick { get; set; }
	private Camera2D camera;
	private MultiplayerSynchronizer multiplayerSynchronizer;
	public bool enableDebug = false;

	public override void _Ready()
	{
		// TODO: why not delete default player and just use mage ? or implement the same speed and health to default player
		var playerClass = new Assassin(); // Instantiate Mage
		Speed = playerClass.Speed; // Override DefaultPlayer's Speed with Mage's Speed
		MaxHealth = playerClass.MaxHealth; // Override DefaultPlayer's MaxHealth with Mage's MaxHealth
		CurrentHealth = playerClass.CurrentHealth; // Set CurrentHealth to Mage's CurrentHealth
		GD.Print($"Mage Speed applied: {Speed}, Mage Health applied: {MaxHealth}");

		AddToGroup("player");
		CurrentHealth = MaxHealth;
		Joystick = GetNode<Node2D>("Joystick");
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
}
