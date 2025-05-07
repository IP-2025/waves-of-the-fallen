using Godot;
using System;
using System.Diagnostics;

public partial class BasicEnemy : EnemyBase
{
	public override void _PhysicsProcess(double delta)
	{
		FindNearestPlayer();
		if (player != null)
		{
			LookAt(player.GlobalPosition);
			Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
			Velocity = direction * speed;
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
