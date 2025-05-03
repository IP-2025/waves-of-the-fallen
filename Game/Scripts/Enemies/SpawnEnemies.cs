using Godot;
using System;

public partial class SpawnEnemies : Node2D
{
	public DefaultPlayer Player { get; set; } // player instance 
	public override void _Ready()
	{
		Timer timer = GetNode<Timer>("SpawnTimer");
		timer.Timeout += OnTimerTimeout;
	}

	private void OnTimerTimeout()
	{
		SpawnEnemy();
	}

	private void SpawnEnemy()
	{
		if (GetTree().GetNodesInGroup("enemies").Count >= 30)
			return;

		bool isRanged = GD.Randf() < 0.4f;

		PackedScene scene = isRanged
			? GD.Load<PackedScene>("res://Scenes/Characters/ranged_enemy.tscn")
			: GD.Load<PackedScene>("res://Scenes/Characters/default_enemy.tscn");

		var enemy = scene.Instantiate<EnemyBase>();
		enemy.player = Player;
		PathFollow2D spawnPath = GetNode<PathFollow2D>("Path2D/PathFollow2D");
		spawnPath.ProgressRatio = GD.Randf();
		enemy.GlobalPosition = spawnPath.GlobalPosition;

		enemy.AddToGroup("enemies");
		if (GetTree().GetNodesInGroup("enemies").Count >= 30)
		{
			return;
		}

		ulong id = enemy.GetInstanceId();
		enemy.Name = $"Enemy_{id}";

		Server.Instance.Entities[(long)id] = enemy;

		AddChild(enemy);
	}

}
