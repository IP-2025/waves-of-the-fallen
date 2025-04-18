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
	
	// New method to avoid obstacles on physics layer 2
public Vector2 AvoidObstacles(Vector2 desiredDirection)
{
	float checkDistance = 32f; // how far ahead to check
	Vector2 checkPosition = GlobalPosition + desiredDirection * checkDistance;

	PhysicsRayQueryParameters2D query = new()
	{
		From = GlobalPosition,
		To = checkPosition,
		CollisionMask = 1 << 1, // Layer 2
		CollideWithAreas = false,
		CollideWithBodies = true
	};

	var spaceState = GetWorld2D().DirectSpaceState;
	var result = spaceState.IntersectRay(query);

	if (result.Count > 0)
	{
		// Obstacle detected – try to steer around it
		Vector2 normal = (Vector2)result["normal"];
		desiredDirection = desiredDirection.Bounce(normal).Normalized();
	}

	return desiredDirection;
}
	
}
