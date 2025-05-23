using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Game.Utilities.Multiplayer;

public partial class SpawnEnemies : Node2D
{
	private WaveTimer waveTimer; // wavetimer so current wave and other functions may be accessed
	private int currentWave; // currentWave, which gets copied from wavetimer for spawning calculations
	private Timer timer; // timer for spawning enemies
	private Dictionary<PackedScene, float> patternPool = new Dictionary<PackedScene, float>(); // dictionary to load and store enemyPatterns
	private int graceTime = 10; // graceTime after each wave
	private int enemyLimit = 10; // enemyLimit, which can scale with currentWave
	private int enemyLimitIncrease = 10; // how much the enemyLimit increases per wave
	private int enemyLimitMax = 30; // the maximum the enemy limit can reach
	public DefaultPlayer Player { get; set; } // player instance 
	private int playerCount;

	public override void _Ready()
	{
		timer = GetNode<Timer>("SpawnTimer");
		timer.Timeout += OnTimerTimeout; // timer event connected
		playerCount = GetTree().GetMultiplayer().GetPeers().Count();
		timer.WaitTime = timer.WaitTime / playerCount; // scale with players
		LoadPatternPool(); // loads the pattern pool see method LoadPatternPool() and debug prints every found pattern and the associated spawning cost
		foreach (KeyValuePair<PackedScene, float> pattern in patternPool)
			Debug.Print("Pattern loaded: " + pattern.Key.Instantiate<Node2D>().SceneFilePath.GetFile() + " with spawningCost: " + pattern.Value);

		waveTimer = FindNodeByType<WaveTimer>(GetTree().Root);
		//waveTimer = GetTree().Root.GetNode<WaveTimer>("GameRoot/WaveTimer"); // loads waveTimer for current wave
		waveTimer.WaveEnded += OnWaveEnd;
		currentWave = waveTimer.waveCounter;

		enemyLimit = Math.Min(enemyLimitIncrease * currentWave, enemyLimitMax); // sets enemy limit, so a custom starting wave can be used at the beginning
		enemyLimitMax += 5 * (playerCount - 1); // increase max ammount of enemies with playerCount
	}

	private T FindNodeByType<T>(Node parent) where T : Node
	{
		foreach (Node child in parent.GetChildren())
		{
			if (child is T t)
				return t;
			var found = FindNodeByType<T>(child);
			if (found != null)
				return found;
		}
		return null;
	}

	private void OnTimerTimeout()
	{
		SpawnEnemiesFromPool(); // spawn enemy on timer timeout
	}

	private void SpawnEnemiesFromPool()
	{
		if (GetTree().GetNodesInGroup("enemies").Count >= enemyLimit) // Checks if the enemy limit is reached and returns if too many enemies are spawned
			return;


		EnemyPattern pattern = GetPatternFromPool();

		pattern.AddToGroup("EnemyPattern"); // adds the pattern to EnemyPattern group, so it can be cleaned up later on

		foreach (EnemyBase enemy in pattern.GetChildren()) // goes through all enemies in the pattern and assigns the player, speed, health and globalposition
		{
			//enemy.player = Player;
			enemy.player = Player;
			enemy.speed = enemy.speed * pattern.speedMultiplier;
			enemy.GetNode<Health>("Health").max_health = enemy.GetNode<Health>("Health").max_health * pattern.healthMultiplier;


			var oldParent = enemy.GetParent();
			oldParent.RemoveChild(enemy);
			enemy.Owner = null;

			SpawnEnemy(enemy);
		}
		pattern.QueueRedraw();
	}


	public void SpawnEnemy(CharacterBody2D enemy, Vector2 spawnPosition) // gives enemy instanceid as name, instantiates on server and adds it to SpawnEnemy
	{
		enemy.GlobalPosition += spawnPosition;

		ulong id = enemy.GetInstanceId();
		enemy.Name = $"Enemy_{id}";
		Server.Instance.Entities[(long)id] = enemy;

		CallDeferred("add_child", enemy);
		enemy.AddToGroup("enemies"); // added to enemy group
	}

	public void SpawnEnemy(CharacterBody2D enemy) // Uses random spawnPath position if called without Vector2
	{
		PathFollow2D spawnPath = GetNode<PathFollow2D>("Path2D/PathFollow2D"); // gets a random starting position, where the enemies are spawned
		spawnPath.ProgressRatio = GD.Randf();
		SpawnEnemy(enemy, spawnPath.GlobalPosition);
	}

	private void LoadPatternPool() // loads patterns through the filepath below into the patternPool with their associated spawningCost
	{
		string patternFilepath = "res://Utilities/Gameflow/Spawn/Patterns/";
		foreach (string patternName in DirAccess.GetFilesAt(patternFilepath))
		{
			PackedScene pattern = GD.Load<PackedScene>(patternFilepath + patternName);
			patternPool.Add(pattern, pattern.Instantiate<EnemyPattern>().spawningCost);
		}
	}

	private void OnWaveEnd() // refreshes the current wave value, calculates the enemy limit, deletes empty patterns and starts a grace time
	{
		currentWave = waveTimer.waveCounter;
		enemyLimit = Math.Min(enemyLimit + enemyLimitIncrease, enemyLimitMax);
		_ = GraceTime();
	}

	private async Task GraceTime() // time in which no additional enemies are spawned and the waveTimer gets paused
	{
		//Debug.Print($"Starting {graceTime} seconds of grace time");
		timer.Paused = true;
		await waveTimer.PauseTimer(graceTime);
		timer.Paused = false;
		//Debug.Print("Grace time has ended");
	}

	private EnemyPattern GetPatternFromPool()
	{

		float spawnValue = GD.Randf() * currentWave; // generate a random spawnValue to determine the difficulty of the selected enemies

		Dictionary<PackedScene, float> patternPoolCopy = patternPool; // copy of patternPool so the loaded patterns don't get removed
		patternPoolCopy = patternPoolCopy.
			Where(i => i.Key.Instantiate<EnemyPattern>().minWave <= currentWave && i.Key.Instantiate<EnemyPattern>().maxWave >= currentWave). // look for patterns in the correct wave number range
			ToDictionary(i => i.Key, i => i.Value);

		patternPoolCopy = patternPoolCopy.
			Where(i => i.Value == patternPoolCopy.FirstOrDefault(i => i.Value > spawnValue, patternPoolCopy.Last()).Value). // looks the next hightest spawncost above spawnValue, if none are found the highest in patternPoolCopy is used
			ToDictionary(i => i.Key, i => i.Value);

		return patternPoolCopy. // instantiates the found pattern as EnemyPattern
			ElementAt((int)(GD.Randf() * patternPoolCopy.Count())). // gets a random remaining pattern from the remaining ones (e.g. if 2 patterns with spawningCost 1 are remaining a random one will be drawn)
			Key.Instantiate<EnemyPattern>();
	}


	// has to be tested if enemypattern nodes exist, can be removed if they never get added to the gameroot
	private void DeleteEmptyPatterns() // cleans up EnemyPattern nodes, which don't have any children so the NodeTree doesn't get cluttered with them
	{
		if (GetTree().GetNodesInGroup("EnemyPattern") != null)
		{
			int counter = 0;
			foreach (Node2D enemy in GetTree().GetNodesInGroup("EnemyPattern")) // deletes every enemy
			{
				//Debug.Print("Deleted enemy");
				if (enemy.GetType() == typeof(EnemyPattern) && enemy.GetChildCount() == 0)
				{
					enemy.QueueFree();
					counter++;
				}
			}
			if (counter != 0) Debug.Print($"Cleaned up {counter} empty pattern nodes");
			else Debug.Print("No empty patterns found");

		}
	}

}