using Godot;
using System;

public partial class DefaultPlayer : CharacterBody2D
{

	public override void _PhysicsProcess(double delta)
	{
		
		// Get the input direction and handle the movement/deceleration.
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		
		Vector2 velocity = direction * 600;
		
		Velocity = velocity;
		
		MoveAndSlide();
		
		var happyBoo = GetNode<Node>("HappyBoo");
		var animationPlayer = happyBoo.GetNode<AnimationPlayer>("AnimationPlayer");

		if (Velocity.Length() > 0.0f)
		{
			animationPlayer.Play("walk");
		}
		else
		{
			animationPlayer.Play("idle");
		}
		
	}
}
