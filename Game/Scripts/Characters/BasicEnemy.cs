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
			
		// Check for obstacles and adjust direction
		direction = AvoidObstacles(direction);
		}
		else
		{
			Velocity = Vector2.Zero;
		}
	

		MoveAndSlide();
	}


	public override void Attack() 
	{
		player.GetNode<Health>("Health").Damage(damage);
		Debug.Print("BasicEnemy attacks (melee)!");
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
