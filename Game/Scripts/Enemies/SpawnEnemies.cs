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

		// Zufällige Auswahl des Gegnertyps
		float randomValue = GD.Randf();
		GD.Print($"Random value: {randomValue}");

		PackedScene scene;
		string enemyType;

		if (randomValue < 0.0001f)
		{
			// RangedEnemy spawnen
			scene = GD.Load<PackedScene>("res://Scenes/Characters/ranged_enemy.tscn");
			enemyType = "RangedEnemy";
		}
		else if (randomValue < 0.7f)
		{
			// MountedEnemy spawnen
			scene = GD.Load<PackedScene>("res://Scenes/Characters/mounted_enemy.tscn");
			enemyType = "MountedEnemy";
		}
		else
		{
			// BasicEnemy spawnen
			scene = GD.Load<PackedScene>("res://Scenes/Characters/default_enemy.tscn");
			enemyType = "BasicEnemy";
		}

		// Gegner instanziieren und konfigurieren
		var enemy = scene.Instantiate<EnemyBase>();
		enemy.player = Player;

		// Spawn-Position festlegen
		PathFollow2D spawnPath = GetNode<PathFollow2D>("Path2D/PathFollow2D");
		spawnPath.ProgressRatio = GD.Randf();
		enemy.GlobalPosition = spawnPath.GlobalPosition;

		enemy.AddToGroup("enemies");
		AddChild(enemy);

		// Log-Ausgabe mit Feindtyp und Gesundheit
		var health = enemy.GetNode<Health>("Health");
		GD.Print($"Spawned enemy: {enemyType} with {health.MaxHealth} health.");
	}
}
