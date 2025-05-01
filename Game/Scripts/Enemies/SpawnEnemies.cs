using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class SpawnEnemies : Node2D
{
	private Dictionary<PackedScene, float> patternPool = new Dictionary<PackedScene, float>();
	public DefaultPlayer Player { get; set; } // player instance 
	public override void _Ready()
	{
		Timer timer = GetNode<Timer>("SpawnTimer");
		timer.Timeout += OnTimerTimeout;
		LoadPatternPool();
		foreach (KeyValuePair<PackedScene, float> pattern in patternPool) Debug.Print("Pattern loaded: " + pattern.Key.ToString() + " with spawningCost: " + pattern.Value);
	}

	private void OnTimerTimeout()
	{
		SpawnEnemy();
	}

	private void SpawnEnemy()
	{
		if (GetTree().GetNodesInGroup("Enemies").Count >= 30)
			return;

		float randFloat = GD.Randf()*6;

		PackedScene scene = randFloat < 0.4f
			? patternPool.FirstOrDefault(x => x.Value > randFloat).Key
			: randFloat < 0.4f ? GD.Load<PackedScene>("res://Scenes/Characters/ranged_enemy.tscn")
			: GD.Load<PackedScene>("res://Scenes/Characters/default_enemy.tscn");

		PathFollow2D spawnPath = GetNode<PathFollow2D>("Path2D/PathFollow2D");
		spawnPath.ProgressRatio = GD.Randf();

		if (randFloat < 0.4f)
		{
			Node2D group = scene.Instantiate<Node2D>();
			group.AddToGroup("Enemies");
			foreach (Node2D e in group.GetChildren())
			{
				EnemyBase enemy = (EnemyBase)e;
				enemy.player = Player;
				group.GlobalPosition = spawnPath.GlobalPosition;
			}
			AddChild(group);
		}
		else
		{
			var enemy = scene.Instantiate<EnemyBase>();
			enemy.player = Player;
			enemy.GlobalPosition = spawnPath.GlobalPosition;

			enemy.AddToGroup("Enemies");

			AddChild(enemy);
		}
	}

	private void LoadPatternPool()
	{
		string patternFilepath = "res://Scenes/Enemies/Patterns/";
		foreach (string patternName in DirAccess.GetFilesAt(patternFilepath))
		{
			PackedScene pattern = GD.Load<PackedScene>(patternFilepath + patternName);
			patternPool.Add(pattern, pattern.Instantiate<EnemyPattern>().spawningCost);
		}
	}

}
