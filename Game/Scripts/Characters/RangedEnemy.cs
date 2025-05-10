using Godot;

/// <summary>
/// Represents a ranged enemy that moves towards the player until a certain distance
/// and attacks the player with projectiles when in range.
/// 
/// Configuration:
/// - stopDistance: Distance at which RangedEnemy stops moving.
/// - attackRange: Short range distance for RangedEnemy's attack.
/// - damage: Damage dealt by the RangedEnemy.
/// - rangedAttackCooldown: Time interval (in seconds) between attacks.
/// - speed: Movement speed of the RangedEnemy.
/// </summary>
public partial class RangedEnemy : EnemyBase
{
	[Export] public float stopDistance = 50f;
	[Export] public float attackRange = 70f;
	[Export] public float damage = 2.5f;
	[Export] public float rangedAttackCooldown = 2.0f;
	[Export] public float speed = 120f;
	[Export] public PackedScene EnemyProjectileScene;

	private float cooldownTimer = 0f;

	protected override float attackCooldown
	{
		get => rangedAttackCooldown;
		set => cooldownTimer = value;
	}

	protected override float timeUntilAttack
	{
		get => cooldownTimer;
		set => cooldownTimer = value;
	}

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

			if (cooldownTimer > 0f)
			{
				cooldownTimer -= (float)delta;
			}

			if (dist <= attackRange && cooldownTimer <= 0f)
			{
				Attack();
				cooldownTimer = rangedAttackCooldown;
			}
		}
	}

	/// <summary>
	/// Instantiates and fires a projectile towards the player.
	/// Ensures the projectile is initialized with the correct direction.
	/// </summary>
	public override void Attack()
	{
		if (EnemyProjectileScene != null && player != null)
		{
			var projectile = (EnemyProjectile)EnemyProjectileScene.Instantiate();
			GetParent().AddChild(projectile);

			projectile.GlobalPosition = GlobalPosition;
			projectile.Initialize(player.GlobalPosition - GlobalPosition, damage); // Übergabe des Schadens

			GD.Print($"RangedEnemy fired a projectile with {damage} damage!");
		}
	}
}
