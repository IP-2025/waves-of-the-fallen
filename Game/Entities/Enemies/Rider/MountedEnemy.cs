using System.Diagnostics;
using Godot;

public partial class MountedEnemy : EnemyBase
{
	/// <summary>
	/// Configuration for the MountedEnemy:
	/// - stopDistance: Distance at which MountedEnemy stops moving.
	/// - attackRange: Close range distance for MountedEnemy's attack.
	/// </summary>
	[Export] public float stopDistance = 200f;
	[Export] public float attackRange = 60f;

	public MountedEnemy()
	{
		speed = 230f;
		damage = 5f;
		attacksPerSecond = 1f;
	}
	public override void _Ready()
	{
		base._Ready();

		// Connect HealthDepleted signal
		var health = GetNode<Health>("Health");
		if (health != null)
		{
			health.HealthDepleted += OnHealthDepleted;
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		var sprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		if (sprite != null && player != null)
		{
			Vector2 toPlayer = (player.GlobalPosition - GlobalPosition).Normalized();
			sprite.FlipH = toPlayer.X > 0;
		}
	}

	/// <summary>
	/// Performs an attack on the player if in range.
	/// </summary>
	public override void Attack()
	{
		if (player != null)
		{
			float dist = GlobalPosition.DistanceTo(player.GlobalPosition);
			if (dist <= attackRange)
			{
				player.GetNode<Health>("Health").Damage(damage);
				if (enableDebug)
					Debug.Print($"MountedEnemy dealt {damage} damage to the player!");
			}
		}
	}

	/// <summary>
	/// Spawns the Rider when the MountedEnemy's health is depleted.
	/// </summary>
	private void OnHealthDepleted()
	{
		if (GetTree().GetMultiplayer().IsServer())
		{
			Velocity = Vector2.Zero;
			animationHandler.SetDeath();

			animationHandler.OnDeathAnimationFinished += () =>
			{
				ActivateRider();
				QueueFree();
			};
		}
	}

	/// <summary>
	/// Activates the Rider by instantiating it and adding it to the scene.
	/// </summary>
	private void ActivateRider()
	{
		PackedScene riderScene = GD.Load<PackedScene>("res://Entities/Enemies/Rider/rider_enemy.tscn");
		if (riderScene == null)
		{
			GD.PrintErr("Failed to load Rider scene!");
			return;
		}

		var riderInstance = riderScene.Instantiate<CharacterBody2D>();
		if (riderInstance == null)
		{
			GD.PrintErr("Failed to instantiate Rider!");
			return;
		}

		var spawnEnemies = GetTree()
		.Root.GetNodeOrNull<GameRoot>("GameRoot")
		?.GetNodeOrNull<SpawnEnemies>("SpawnEnemies");

		if (spawnEnemies != null)
		{
			spawnEnemies.SpawnEnemy(riderInstance, GlobalPosition);
		}
		else
		{
			Debug.Print("Failed to find SpawnEnemies in GameRoot");
		}
	}
}
