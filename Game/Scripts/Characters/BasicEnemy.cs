using Godot;
using System;
using System.Diagnostics;

public partial class BasicEnemy : EnemyBase
{
	public override void _PhysicsProcess(double delta)
	{
		if (currentState == EnemyAnimationState.Die)
		{
			Velocity = Vector2.Zero;
			MoveAndSlide();
			return;
		}

		FindNearestPlayer();
		
		if (player != null)
		{
			Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
			Velocity = direction * speed;
			if (animation != null)
			{
				animation.FlipH = direction.X < 0;
			}
		}
		else
		{
			Velocity = Vector2.Zero;
		}

		MoveAndSlide();
		UpdateAnimationState();
	}

	public override void Attack() 
	{
		player.GetNode<Health>("Health").Damage(damage);
		Debug.Print("BasicEnemy attacks (melee)!");
	}
}
