using Godot;

public partial class RangedEnemy : EnemyBase
{
	[Export] public float stopDistance = 350f;
	[Export] public float attackRange = 300f;
	[Export] public PackedScene EnemyProjectileScene;

	private float cooldownTimer = 0f;

	public RangedEnemy()
	{
		speed = 100f;
		damage = 2.5f;
		attacksPerSecond = 0.8f;
	}

	protected override void HandleMovement(Vector2 direction)
	{
		float dist = GlobalPosition.DistanceTo(player.GlobalPosition);

		if (dist > stopDistance)
		{
			Velocity = (player.GlobalPosition - GlobalPosition).Normalized() * speed;
		}
		else
		{
			Velocity = Vector2.Zero;

			if (cooldownTimer > 0f)
			{
				cooldownTimer -= (float)GetProcessDeltaTime();
			}

			if (dist <= attackRange && cooldownTimer <= 0f)
			{
				Attack();
				cooldownTimer = attackCooldown;
			}
		}
	}

	/// <summary>
	/// Spawns and fires a projectile towards the player.
	/// </summary>
	public override void Attack()
	{
		if (EnemyProjectileScene != null && player != null)
		{
			var projectile = (EnemyProjectile)EnemyProjectileScene.Instantiate();
			GetParent().AddChild(projectile);

			projectile.GlobalPosition = GlobalPosition;
			projectile.Initialize(player.GlobalPosition - GlobalPosition, damage);

			if (enableDebug)
			{
				GD.Print($"RangedEnemy attacks with damage: {damage}");
			}
		}
	}
}
