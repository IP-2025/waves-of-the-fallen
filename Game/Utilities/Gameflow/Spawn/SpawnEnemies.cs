using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Game.Utilities.Multiplayer;

public partial class SpawnEnemies : Node2D
{
	private WaveTimer _waveTimer; // wavetimer so current wave and other functions may be accessed
	private int _currentWave; // currentWave, which gets copied from wavetimer for spawning calculations
	private Timer _timer; // timer for spawning enemies
	private readonly Dictionary<PackedScene, float> _patternPool = new(); // dictionary to load and store enemyPatterns
	private int _graceTime = 10; // graceTime after each wave
	private int _enemyLimit = 10; // enemyLimit, which can scale with currentWave
	private int _enemyLimitIncrease = 10; // how much the enemyLimit increases per wave
	private int _enemyLimitMax = 30; // the maximum the enemy limit can reach
	private DefaultPlayer Player { get; set; } // player instance 
	private int _playerCount;

	public override void _Ready()
	{
		_timer = GetNode<Timer>("SpawnTimer");
		_timer.Timeout += OnTimerTimeout; // timer event connected
		_playerCount = GetTree().GetMultiplayer().GetPeers().Count();
		_timer.WaitTime = _timer.WaitTime / _playerCount; // scale with players
		LoadPatternPool(); // loads the pattern pool see method LoadPatternPool() and debug prints every found pattern and the associated spawning cost
		foreach (var pattern in _patternPool)
			Debug.Print("Pattern loaded: " + pattern.Key.Instantiate<Node2D>().SceneFilePath.GetFile() + " with spawningCost: " + pattern.Value);

		_waveTimer = FindNodeByType<WaveTimer>(GetTree().Root);
		//waveTimer = GetTree().Root.GetNode<WaveTimer>("GameRoot/WaveTimer"); // loads waveTimer for current wave
		_waveTimer.WaveEnded += OnWaveEnd;
		_currentWave = _waveTimer.WaveCounter;

		_enemyLimit = Math.Min(_enemyLimitIncrease * _currentWave, _enemyLimitMax); // sets enemy limit, so a custom starting wave can be used at the beginning
		_enemyLimitMax += 5 * (_playerCount - 1); // increase max ammount of enemies with playerCount
	}

	private T FindNodeByType<T>(Node parent) where T : Node
	{
		foreach (var child in parent.GetChildren())
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
		if (GetTree().GetNodesInGroup("enemies").Count >= _enemyLimit) // Checks if the enemy limit is reached and returns if too many enemies are spawned
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

		var id = enemy.GetInstanceId();
		enemy.Name = $"Enemy_{id}";
		Server.Instance.Entities[(long)id] = enemy;

		CallDeferred("add_child", enemy);
		enemy.AddToGroup("enemies"); // added to enemy group
	}

	private void SpawnEnemy(CharacterBody2D enemy) // Uses random spawnPath position if called without Vector2
	{
		PathFollow2D spawnPath = GetNode<PathFollow2D>("Path2D/PathFollow2D"); // gets a random starting position, where the enemies are spawned
		spawnPath.ProgressRatio = GD.Randf();
		SpawnEnemy(enemy, spawnPath.GlobalPosition);
	}

	private void LoadPatternPool() // loads patterns through the filepath below into the patternPool with their associated spawningCost
	{
		const string patternFilepath = "res://Utilities/Gameflow/Spawn/Patterns/";
		foreach (var patternName in DirAccess.GetFilesAt(patternFilepath))
		{
			PackedScene pattern = GD.Load<PackedScene>(patternFilepath + patternName);
			_patternPool.Add(pattern, pattern.Instantiate<EnemyPattern>().spawningCost);
		}
	}

	private void OnWaveEnd() // refreshes the current wave value, calculates the enemy limit, deletes empty patterns and starts a grace time
	{
		_currentWave = _waveTimer.WaveCounter;
		_enemyLimit = Math.Min(_enemyLimit + _enemyLimitIncrease, _enemyLimitMax);
		_ = GraceTime();
	}

	private async Task GraceTime() // time in which no additional enemies are spawned and the waveTimer gets paused
	{
		//Debug.Print($"Starting {graceTime} seconds of grace time");
		_timer.Paused = true;
		await _waveTimer.PauseTimer(_graceTime);
		_timer.Paused = false;
		//Debug.Print("Grace time has ended");
	}

	private EnemyPattern GetPatternFromPool()
	{

		var spawnValue = GD.Randf() * _currentWave; // generate a random spawnValue to determine the difficulty of the selected enemies

		var patternPoolCopy = _patternPool; // copy of patternPool so the loaded patterns don't get removed
		patternPoolCopy = patternPoolCopy.
			Where(i => i.Key.Instantiate<EnemyPattern>().minWave <= _currentWave && i.Key.Instantiate<EnemyPattern>().maxWave >= _currentWave). // look for patterns in the correct wave number range
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
		if (GetTree().GetNodesInGroup("EnemyPattern") == null) return;
		var counter = 0;
		foreach (Node2D enemy in GetTree().GetNodesInGroup("EnemyPattern")) // deletes every enemy
		{
			//Debug.Print("Deleted enemy");
			if (enemy.GetType() != typeof(EnemyPattern) || enemy.GetChildCount() != 0) continue;
			enemy.QueueFree();
			counter++;
		}

		Debug.Print(counter != 0 ? $"Cleaned up {counter} empty pattern nodes" : "No empty patterns found");
	}

}