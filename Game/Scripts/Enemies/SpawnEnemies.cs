using Godot;
using System;

public partial class SpawnEnemies : Node2D
{
	public DefaultPlayer Player { get; set; } // player instance 

	public override void _Ready()
	{
		Timer timer = GetNode<Timer>("SpawnTimer");
		timer.Timeout += OnTimerTimeout; // timer event connected
	}

	private void OnTimerTimeout()
	{
		SpawnEnemy(); // spawn enemy on timer timeout
	}

	private void SpawnEnemy()
	{
		if (GetTree().GetNodesInGroup("enemies").Count >= 30)
			return; // limit reached, no more enemies

		// random enemy type selection
		float randomValue = GD.Randf();
		GD.Print($"Random value: {randomValue}");

		PackedScene scene;
		string enemyType;

		if (randomValue < 0.4f)
		{
			// spawn ranged enemy
			scene = GD.Load<PackedScene>("res://Scenes/Characters/ranged_enemy.tscn");
			enemyType = "RangedEnemy";
		}
		else if (randomValue < 0.7f)
		{
			// spawn mounted enemy
			scene = GD.Load<PackedScene>("res://Scenes/Characters/mounted_enemy.tscn");
			enemyType = "MountedEnemy";
		}
		else
		{
			// spawn basic enemy
			scene = GD.Load<PackedScene>("res://Scenes/Characters/default_enemy.tscn");
			enemyType = "BasicEnemy";
		}

		// instantiate and configure enemy
		var enemy = scene.Instantiate<EnemyBase>();
		enemy.player = Player;

		// spawn-position defined
		PathFollow2D spawnPath = GetNode<PathFollow2D>("Path2D/PathFollow2D");
		spawnPath.ProgressRatio = GD.Randf();
		enemy.GlobalPosition = spawnPath.GlobalPosition;

		enemy.AddToGroup("enemies"); // added to enemies group

		ulong id = enemy.GetInstanceId();
		enemy.Name = $"Enemy_{id}";

		Server.Instance.Entities[(long)id] = enemy;

		AddChild(enemy); // added to scene

		// log enemy type and health
		var health = enemy.GetNode<Health>("Health");
		GD.Print($"Spawned enemy: {enemyType} with {health.MaxHealth} health.");
	}
}
