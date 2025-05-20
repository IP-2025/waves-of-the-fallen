using Godot;

public partial class MountedEnemy : EnemyBase
{
	/// <summary>
	/// Configuration for the MountedEnemy:
	/// - stopDistance: Distance at which MountedEnemy stops moving.
	/// - attackRange: Close range distance for MountedEnemy's attack.
	/// </summary>
	[Export] public float stopDistance = 15f;
	[Export] public float attackRange = 10f;

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
			GD.Print($"MountedEnemy dealt {damage} damage to the player!");
		}
	}

	/// <summary>
	/// Spawns the Rider when the MountedEnemy's health is depleted.
	/// </summary>
	private void OnHealthDepleted()
	{
		ActivateRider();
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

		riderInstance.Visible = true;
		// add rider deffered in next frame to prevent errors
		GetParent().CallDeferred("add_child", riderInstance);
		// check deffered if rider was added in next frame
		CallDeferred(nameof(CheckRiderAdded), riderInstance);
		riderInstance.GlobalPosition = GlobalPosition;
	}
		

	private void CheckRiderAdded(CharacterBody2D rider)
	{
		if (rider.GetParent() == null)
			GD.PrintErr("Rider was not added to the scene!");
	}
}
