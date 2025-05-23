using System.Diagnostics;
using Godot;

public partial class Rider : EnemyBase
{
	/// <summary>
	/// Configuration for the Rider:
	/// - stopDistance: Distance at which Rider stops moving.
	/// - attackRange: Close range distance for Rider's attack.
	/// </summary>
	[Export] public float stopDistance = 20f;
	[Export] public float attackRange = 10f;

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

	/// <summary>
	/// Performs an attack on the player if in range.
	/// </summary>
	public override void Attack()
	{
		if (player != null)
		{
			player.GetNode<Health>("Health").Damage(damage);
			if (enableDebug)
				Debug.Print($"Rider dealt {damage} damage to the player!");
		}
	}

}
