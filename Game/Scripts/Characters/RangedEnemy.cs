using Godot;

/// <summary>
/// RangedEnemy configuration:
/// - Standard values (speed, damage, attacksPerSecond) are defined in the constructor.
/// - stopDistance: Distance at which the RangedEnemy stops moving towards the player.
/// - attackRange: Maximum range within which the RangedEnemy can attack.
/// </summary>
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

	public override void _Ready()
	{
		base._Ready();

		if (enableDebug)
		{
			GD.Print($"RangedEnemy initialized with speed: {speed}, damage: {damage}, attacksPerSecond: {attacksPerSecond}");
		}
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

			// Attack is only executed in the base class
		}
	}

	public override void Attack()
	{
		if (EnemyProjectileScene != null && player != null)
		{
			if (enableDebug)
			{
				GD.Print($"{Name} is attacking. CooldownTimer: {timeUntilAttack}");
			}

			var projectile = (EnemyProjectile)EnemyProjectileScene.Instantiate();
			GetParent().AddChild(projectile);

			projectile.GlobalPosition = GlobalPosition;
			projectile.Initialize(player.GlobalPosition - GlobalPosition, damage);

			if (enableDebug)
			{
				GD.Print($"{Name} attacks with damage: {damage}");
			}
		}
	}
}
