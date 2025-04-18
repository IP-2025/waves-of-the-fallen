using Godot;
using System;

public partial class RangedEnemy : EnemyBase
{
	[Export] public float stopDistance = 350f;

	public override void _PhysicsProcess(double delta)
	{
		FindNearestPlayer();
		if (player == null)
		{
			Velocity = Vector2.Zero;
			MoveAndSlide();
			return;
		}

		float dist = GlobalPosition.DistanceTo(player.GlobalPosition);
		LookAt(player.GlobalPosition);
		
		if (dist > stopDistance)
		{
			Vector2 toPlayer = (player.GlobalPosition - GlobalPosition).Normalized();
			Velocity = toPlayer * speed;
		}
		else
		{
			Velocity = Vector2.Zero;
		}

		MoveAndSlide();
	}

	public override void Attack()
	{
		GD.Print("RangedEnemy attacks from distance!");
	}
		
}
