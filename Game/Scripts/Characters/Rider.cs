using Godot;

public partial class Rider : EnemyBase
{
	[Export] public float stopDistance = 100f; // distance at which Rider stops moving

	public override void _PhysicsProcess(double delta)
	{
		FindNearestPlayer(); // find the closest player
		if (player == null)
		{
			Velocity = Vector2.Zero; // stop moving if no player is found
			MoveAndSlide();
			return;
		}

		float dist = GlobalPosition.DistanceTo(player.GlobalPosition); // calculate distance to player
		LookAt(player.GlobalPosition); // face the player

		if (dist > stopDistance)
		{
			// move towards the player
			Vector2 toPlayer = (player.GlobalPosition - GlobalPosition).Normalized();
			Velocity = toPlayer * speed;
		}
		else
		{
			Velocity = Vector2.Zero; // stop moving when within stop distance
		}

		MoveAndSlide(); // apply movement
	}

	public override void Attack()
	{
		GD.Print("Rider attacks from distance!"); // debug message for attack
	}
}
