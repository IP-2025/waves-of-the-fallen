using Godot;

public partial class MountedEnemy : EnemyBase
{
	[Export] public float stopDistance = 200f; // Distanz, bei der der MountedEnemy stehen bleibt
	[Export] public PackedScene BasicEnemyScene; // Szene für den BasicEnemy

	public override void _Ready()
	{
		base._Ready();

		// Überprüfe, ob die BasicEnemy-Szene korrekt zugewiesen ist
		if (BasicEnemyScene == null)
		{
			GD.PrintErr("BasicEnemyScene is not assigned in MountedEnemy!");
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		FindNearestPlayer();
		if (player == null)
		{
			Velocity = Vector2.Zero;
			MoveAndSlide();
			return;
		}

		float dist = GlobalPosition.DistanceTo(player.GlobalPosition);
		LookAt(player.GlobalPosition);

		if (dist > stopDistance)
		{
			Vector2 toPlayer = (player.GlobalPosition - GlobalPosition).Normalized();
			Velocity = toPlayer * 400f; // Feste Geschwindigkeit für MountedEnemy
		}
		else
		{
			Velocity = Vector2.Zero;
		}

		MoveAndSlide();
	}

	public override void Attack()
	{
		GD.Print("MountedEnemy attacks!");
	}

	public void OnHealthDepleted()
	{
		GD.Print("MountedEnemy has been defeated!");
		SpawnBasicEnemy(); // Spawne einen BasicEnemy
		QueueFree(); // Entferne den MountedEnemy aus der Szene
	}

	private void SpawnBasicEnemy()
	{
		if (BasicEnemyScene == null)
		{
			GD.PrintErr("BasicEnemyScene is not assigned!");
			return;
		}

		var basicEnemy = BasicEnemyScene.Instantiate<EnemyBase>();
		basicEnemy.GlobalPosition = GlobalPosition; // Setze die Position des BasicEnemy auf die Position des MountedEnemy
		GetParent().AddChild(basicEnemy); // Füge den BasicEnemy zur Szene hinzu

		GD.Print($"BasicEnemy spawned at position {basicEnemy.GlobalPosition}");
	}
}
