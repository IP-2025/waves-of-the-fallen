using Godot;
using System;

public partial class RangedEnemy : EnemyBase
{
	[Export] public float stopDistance = 350f;

	protected override void HandleMovement(Vector2 direction)
	{
		float dist = GlobalPosition.DistanceTo(player.GlobalPosition);
		if (dist > stopDistance)
		{
			Vector2 toPlayer = (player.GlobalPosition - GlobalPosition).Normalized();
			Velocity = toPlayer * speed;
		}
		else
		{
			Velocity = Vector2.Zero;
		}
	}

	public override void Attack()
	{
		GD.Print("RangedEnemy attacks from distance!");
	}
}
