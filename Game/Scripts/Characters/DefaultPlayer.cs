using Godot;
using System;

public partial class DefaultPlayer : CharacterBody2D
{
	[Export]
	public float Speed = 600.0f;

	private Node2D _joystick;
	private AnimationPlayer _animationPlayer;
	private Sprite2D _archerSprite;

	public override void _Ready()
	{
		_joystick = GetNode<Node2D>("./Joystick");
		
		_animationPlayer = GetNode<AnimationPlayer>("./Archer/ArcherAnimation");
		
		_archerSprite = GetNode<Sprite2D>("./Archer");
		
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Vector2.Zero;
		
		if (_joystick != null)
		{
			Vector2 joystickDirection = (Vector2)_joystick.Get("PosVector");
			if (joystickDirection != Vector2.Zero)
			{
				direction = joystickDirection;
			}
		}

		
		if (direction == Vector2.Zero)
		{
			direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		}

		Velocity = direction * Speed;
		MoveAndSlide();
		
		if (_animationPlayer != null)
		{
			if (direction != Vector2.Zero)
			{
				if (!_animationPlayer.IsPlaying() || _animationPlayer.CurrentAnimation != "walk")
					_animationPlayer.Play("walk");
			}
			else
			{
				if (!_animationPlayer.IsPlaying() || _animationPlayer.CurrentAnimation != "idle")
					_animationPlayer.Play("idle");
			}
		}
		
		if (_archerSprite != null && direction != Vector2.Zero)
		{
			_archerSprite.FlipH = direction.X < 0;
		}
		
	}
	
}
