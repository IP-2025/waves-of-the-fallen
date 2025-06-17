using Godot;

/// <summary>
/// Represents an enemy projectile that moves in a given direction,
/// detects collisions, and applies damage to the player.
/// 
/// Configuration:
/// - Speed: Movement speed of the projectile.
/// - Damage: Damage dealt to the player upon collision.
/// - Lifetime: Time before the projectile is automatically removed.
/// 
/// Behavior:
/// - Moves in the initialized direction.
/// - Detects collisions with the player and applies damage.
/// - Removes itself after hitting the player or exceeding its lifetime.
/// </summary>
public partial class EnemyProjectile : Area2D
{
	[Export] public float Speed = 300.0f;
	[Export] public float Lifetime = 5.0f;
	private float Damage;
	private Vector2 direction;
	private float lifetimeTimer = 0.0f;

	public void Initialize(Vector2 dir, float damage, float speed)
	{
		direction = dir.Normalized();
		Damage = damage;
		Speed = speed;

		// Set rotation based on direction
		Rotation = direction.Angle();
	}

	public override void _Ready()
	{
		Connect("body_entered", new Callable(this, nameof(_on_EnemyProjectile_body_entered)));
	}

	public override void _PhysicsProcess(double delta)
	{
		Position += direction * Speed * (float)delta;
		lifetimeTimer += (float)delta;

		if (lifetimeTimer >= Lifetime)
		{
			QueueFree();
		}
	}

	/// <summary>
	/// Handles collision with other objects.
	/// Applies damage to the player or shield if hit and removes the projectile.
	/// </summary>
	private void _on_EnemyProjectile_body_entered(Node body)
	{
		if (body is DefaultPlayer player)
		{
			var health = player.GetNodeOrNull<Health>("Health");
			if (health != null)
			{
				health.Damage(Damage);
			}
			QueueFree();
		}
		else if (body is Shield shield)
		{
			var health = shield.GetNodeOrNull<Health>("Health");
			if (health != null)
			{
				health.Damage(Damage);
			}
			QueueFree();
		}
		else if (body is StaticBody2D || body is TileMap)
		{
			QueueFree();
		}
	}
}
