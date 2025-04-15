using Godot;
using System;
using System.Diagnostics;

public partial class BasicEnemy : EnemyBase
{
	public override void _PhysicsProcess(double delta)
	{
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
	}

	public override void Attack()
	{
		Debug.Print("BasicEnemy attacks (melee)!");
	}
}
