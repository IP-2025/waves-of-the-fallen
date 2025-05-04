using Godot;

public partial class RangedEnemy : EnemyBase
{
	/// <summary>
	/// Configuration for the RangedEnemy:
	/// - stopDistance: Distance at which RangedEnemy stops moving.
	/// - attackRange: Short range distance for RangedEnemy's attack.
	/// - damage: Damage dealt by the RangedEnemy.
	/// - attackCooldown: Cooldown time between attacks (in seconds).
	/// - speed: Movement speed of the RangedEnemy.
	/// </summary>
	[Export] public float stopDistance = 50f;
	[Export] public float attackRange = 30f;
	[Export] public float damage = 1f;
	[Export] public float attackCooldown = 2.0f;
	[Export] public float speed = 120f;

	private float attackTimer = 0f;

	public override void _PhysicsProcess(double delta)
	{
		// Find the nearest player
		FindNearestPlayer();
		if (player == null)
		{
			Velocity = Vector2.Zero;
			MoveAndSlide();
			return;
		}

		// Calculate distance to the player
		float dist = GlobalPosition.DistanceTo(player.GlobalPosition);

		// Face the player
		LookAt(player.GlobalPosition);

		// Move towards the player if outside stopDistance
		if (dist > stopDistance)
		{
			Vector2 toPlayer = (player.GlobalPosition - GlobalPosition).Normalized();
			Velocity = toPlayer * speed;
		}
		else
		{
			Velocity = Vector2.Zero;

			// Attack the player if within attackRange and cooldown is over
			if (dist <= attackRange && attackTimer <= 0f)
			{
				Attack();
				attackTimer = attackCooldown;
			}
		}

		// Reduce the attack cooldown timer
		if (attackTimer > 0f)
		{
			attackTimer -= (float)delta;
		}

		// Apply movement
		MoveAndSlide();
	}

	/// <summary>
	/// Performs an attack on the player if in range.
	/// </summary>
	public override void Attack()
	{
		if (player != null)
		{
			player.GetNode<Health>("Health").Damage(damage);
			GD.Print($"RangedEnemy dealt {damage} damage to the player!");
		}
	}
}
