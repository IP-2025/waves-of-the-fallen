using Godot;
using System;

public partial class RangedEnemy : EnemyBase
{
	[Export] public float stopDistance = 350f;
	[Export] public float attackRange = 300f;
	[Export] public float damage = 2.5f;
	[Export] public float rangedAttackCooldown = 2.0f;
	[Export] public PackedScene EnemyProjectileScene;

	private float cooldownTimer = 0f;

	protected override void HandleMovement(Vector2 direction)
	{
		float dist = GlobalPosition.DistanceTo(player.GlobalPosition);

		if (dist > stopDistance)
		{
			// Move towards the player
			Vector2 toPlayer = (player.GlobalPosition - GlobalPosition).Normalized();
			Velocity = toPlayer * speed;
		}
		else
		{
			// Stop moving and handle attack logic
			Velocity = Vector2.Zero;

			if (cooldownTimer > 0f)
			{
				cooldownTimer -= (float)GetProcessDeltaTime();
			}

			if (dist <= attackRange && cooldownTimer <= 0f)
			{
				Attack();
				cooldownTimer = rangedAttackCooldown;
			}
		}
	}

	public override void Attack()
	{
		if (EnemyProjectileScene != null && player != null)
		{
			// Instantiate and fire a projectile
			var projectile = (EnemyProjectile)EnemyProjectileScene.Instantiate();
			GetParent().AddChild(projectile);

			projectile.GlobalPosition = GlobalPosition;

			// Initialize the projectile with direction and damage
			projectile.Initialize(player.GlobalPosition - GlobalPosition, damage);
		}
	}
}
