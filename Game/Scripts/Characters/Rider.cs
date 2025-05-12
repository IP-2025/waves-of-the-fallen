using Godot;

public partial class Rider : EnemyBase
{
	/// <summary>
	/// Configuration for the Rider:
	/// - stopDistance: Distance at which Rider stops moving.
	/// - attackRange: Close range distance for Rider's attack.
	/// - damage: Damage dealt by the Rider.
	/// - attackCooldown: Cooldown time between attacks (in seconds).
	/// - speed: Movement speed of the Rider.
	/// </summary>
	[Export] public float stopDistance = 20f;
	[Export] public float attackRange = 10f;
	[Export] public float damage = 5f;
	[Export] public float attackCooldown = 1.0f;
	[Export] public float speed = 150f;

	private float attackTimer = 0f;

	public override void _PhysicsProcess(double delta)
	{
		FindNearestPlayer();
		if (player == null)
		{
			Velocity = Vector2.Zero;
			PauseAnimation(); // Pause Walk animation when no player is found
			MoveAndSlide();
			return;
		}

		float dist = GlobalPosition.DistanceTo(player.GlobalPosition);

		LookAt(player.GlobalPosition);

		if (dist > stopDistance)
		{
			Vector2 toPlayer = (player.GlobalPosition - GlobalPosition).Normalized();
			Velocity = toPlayer * speed;

			// Play Walk animation when moving
			PlayAnimation();
		}
		else
		{
			Velocity = Vector2.Zero;

			// Pause Walk animation when stopping
			PauseAnimation();

			if (dist <= attackRange && attackTimer <= 0f)
			{
				Attack();
				attackTimer = attackCooldown;
			}
		}

		if (attackTimer > 0f)
		{
			attackTimer -= (float)delta;
		}

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
			GD.Print($"Rider dealt {damage} damage to the player!");

			// Pause Walk animation during attack (optional)
			PauseAnimation();
		}
	}

	/// <summary>
	/// Plays the Walk animation on the AnimatedSprite2D node.
	/// </summary>
	private void PlayAnimation()
	{
		var animatedSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		if (animatedSprite != null && !animatedSprite.IsPlaying())
		{
			animatedSprite.Play("Walk");
		}
	}

	/// <summary>
	/// Pauses the Walk animation on the AnimatedSprite2D node.
	/// </summary>
	private void PauseAnimation()
	{
		var animatedSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		if (animatedSprite != null)
		{
			animatedSprite.Stop();
		}
	}
}
