using Godot;
using System;

public partial class SpawnEnemies : Node2D
{
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
		var enemy = GD.Load<PackedScene>("res://Scenes/Characters/default_enemy.tscn").Instantiate<BasicEnemy>();
		PathFollow2D spawnPath = GetNode<PathFollow2D>("Path2D/PathFollow2D");
		spawnPath.ProgressRatio = GD.Randf();
		enemy.GlobalPosition = spawnPath.GlobalPosition;
		
		// following 5 lines only here to limit spawn rate, can be adapted later 
		enemy.AddToGroup("enemies");
		if (GetTree().GetNodesInGroup("enemies").Count >= 30)
		{
			return;
		}
		
		AddChild(enemy);
	}

}
