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
		if (GetTree().GetNodesInGroup("enemies").Count >= 30)
			return;

		bool isRanged = GD.Randf() < 0.4f;

		PackedScene scene = isRanged
			? GD.Load<PackedScene>("res://Scenes/Characters/ranged_enemy.tscn")
			: GD.Load<PackedScene>("res://Scenes/Characters/default_enemy.tscn");

		var enemy = scene.Instantiate<CharacterBody2D>();
		PathFollow2D spawnPath = GetNode<PathFollow2D>("Path2D/PathFollow2D");
		spawnPath.ProgressRatio = GD.Randf();
		enemy.GlobalPosition = spawnPath.GlobalPosition;

		enemy.AddToGroup("enemies");
		AddChild(enemy);
	}

}
