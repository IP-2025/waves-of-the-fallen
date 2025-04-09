using Godot;
using System;

public partial class DefaultPlayer : CharacterBody2D
{
	[Export]
	public float Speed = 600.0f;

	private Node2D _joystick;

	public override void _Ready()
	{
		// Find the joystick
		_joystick = GetNode<Node2D>("./Joystick");
		
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Vector2.Zero;

		// Check if joystick exists and if it's being used
		if (_joystick != null)
		{
			Vector2 joystickDirection = (Vector2)_joystick.Get("PosVector");
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
	
}

