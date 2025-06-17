using Godot;

/// <summary>
/// RangedEnemy configuration:
/// - Standard values (speed, damage, attacksPerSecond) are defined in the constructor.
/// - stopDistance: Distance at which the RangedEnemy stops moving towards the player.
/// - attackRange: Maximum range within which the RangedEnemy can attack.
/// </summary>
public partial class RangedEnemy : EnemyBase
{
	[Export] public float stopDistance = 150f;
	[Export] public float attackRange = 300f;
	[Export] public PackedScene EnemyProjectileScene;

	private float cooldownTimer = 0f;

	public RangedEnemy()
	{
		speed = 250;
		damage = 2.5f;
		attacksPerSecond = 0.8f;
		scoreValue = 30;
	}

	public override void _Ready()
	{
		base._Ready();
		if (enableDebug)
		{
			GD.Print($"RangedEnemy initialized with speed: {speed}, damage: {damage}, attacksPerSecond: {attacksPerSecond}");
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
			projectile.Initialize(player.GlobalPosition - GlobalPosition, damage, speed+(speed/5f));

			if (enableDebug)
			{
				GD.Print($"{Name} attacks with damage: {damage}");
			}
		}
	}
}
