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
	private int _enemyLimitIncrease = 5; // how much the enemyLimit increases per wave
	private int _enemyLimitMax = 45; // the maximum the enemy limit can reach
	private DefaultPlayer Player { get; set; } // player instance 
	private int _playerCount;
	private float _healthMultiplier = 1;
	private float _damageMultiplier = 1;
	private float _attackSpeedMultiplier = 1;
	private float _moveSpeedMultiplier = 1;

	public override void _Ready()
	{
		_timer = GetNode<Timer>("SpawnTimer");
		_timer.Timeout += OnTimerTimeout; // timer event connected
		_playerCount = GetTree().GetMultiplayer().GetPeers().Count();
		_playerCount = _playerCount <= 0 ? 1 : _playerCount; // have at least one player, (important for solo mode without multiplayer peers)
		_timer.WaitTime = _timer.WaitTime / _playerCount; // scale with players
		LoadPatternPool(); // loads the pattern pool see method LoadPatternPool() and debug prints every found pattern and the associated spawning cost
		foreach (var pattern in _patternPool)
			Debug.Print("Pattern loaded: " + pattern.Key.Instantiate<Node2D>().SceneFilePath.GetFile() + " with spawningCost: " + pattern.Value);

		_waveTimer = FindNodeByType<WaveTimer>(GetTree().Root);
		//waveTimer = GetTree().Root.GetNode<WaveTimer>("GameRoot/WaveTimer"); // loads waveTimer for current wave
		_waveTimer.WaveEnded += OnWaveEnd;
		_currentWave = _waveTimer.WaveCounter;
		_waveTimer.WaveStarted += OnWaveStart;

		_enemyLimit = Math.Min(_enemyLimitIncrease * _currentWave, _enemyLimitMax); // sets enemy limit, so a custom starting wave can be used at the beginning
		_enemyLimitMax += 10 * (_playerCount - 1); // increase max ammount of enemies with playerCount

		// initialize scaling values for higher wave start. all values scale additively (wave 1 health = *1 multiplier | wave 10 health = *1.9 multiplier)
		_healthMultiplier = 1f + (_currentWave - 1) * (0.1f + (_playerCount-1)*0.05f); // increases enemy health by 10%
		_damageMultiplier = 1f + (_currentWave - 1) * (0.2f + (_playerCount-1)*0.1f); // increases enemy damage by 20%
		_attackSpeedMultiplier = Math.Min(1f + (_currentWave - 1) * 0.05f , 2f); // increases enemy attack speed by 5% until 200% is reached
		_moveSpeedMultiplier = Math.Min(1f + (_currentWave - 1) * 0.0375f , 1.5f); // increases enemy movement speed by 5% until 200% is reached

	}
	private void spawnGiantBoss()
	{
		PackedScene packedScene = GD.Load<PackedScene>("res://Utilities/Gameflow/Spawn/BossPatterns/giant_boss.tscn");
		EnemyPattern pattern = packedScene.Instantiate<EnemyPattern>();
		PathFollow2D spawnPath = GetNode<PathFollow2D>("Path2D/PathFollow2D"); // gets a random starting position, where the enemies are spawned
		spawnPath.ProgressRatio = GD.Randf();

		pattern.healthMultiplier = 1 + (_currentWave / 5);

		spawnPattern(pattern);

		pattern.QueueRedraw();
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

		spawnPattern(pattern);

		pattern.QueueRedraw();

	}

	private void spawnPattern(EnemyPattern pattern)
	{
		pattern.AddToGroup("EnemyPattern"); // adds the pattern to EnemyPattern group, so it can be cleaned up later on

		foreach (EnemyBase enemy in pattern.GetChildren()) // goes through all enemies in the pattern and assigns the player, speed, health and globalposition
		{
			//enemy.player = Player;
			enemy.player = Player;
			enemy.speed = enemy.speed * pattern.speedMultiplier;
			enemy.GetNode<Health>("Health").max_health = enemy.GetNode<Health>("Health").max_health * pattern.healthMultiplier;
			//pattern.GlobalPosition = spawnPath.GlobalPosition;
		   // enemy.GlobalPosition += spawnPath.GlobalPosition;

		  //  ulong id = enemy.GetInstanceId();
		 //   enemy.Name = $"Enemy_{id}";
		 //   Server.Instance.Entities[(long)id] = enemy;

			var oldParent = enemy.GetParent();
			oldParent.RemoveChild(enemy);
			enemy.Owner = null;

			SpawnEnemy(enemy);
		}
		pattern.QueueRedraw();
	}


	public void SpawnEnemy(EnemyBase enemy, Vector2 spawnPosition) // gives enemy instanceid as name, instantiates on server and adds it to SpawnEnemy
	{
		enemy.GlobalPosition += spawnPosition;

		// wavecount based scaling
		enemy.GetNode<Health>("Health").max_health *= _healthMultiplier;
		enemy.damage *= _damageMultiplier;
		enemy.speed *= _moveSpeedMultiplier;
		enemy.attacksPerSecond *= _attackSpeedMultiplier;

		var id = enemy.GetInstanceId();
		enemy.Name = $"Enemy_{id}";
		Server.Instance.Entities[(long)id] = enemy;

		CallDeferred("add_child", enemy);
		enemy.AddToGroup("enemies"); // added to enemy group
	}

	private void SpawnEnemy(EnemyBase enemy) // Uses random spawnPath position if called without Vector2
	{
		PathFollow2D spawnPath = GetNode<PathFollow2D>("Path2D/PathFollow2D"); // gets a random starting position, where the enemies are spawned
		spawnPath.ProgressRatio = GD.Randf();
		SpawnEnemy(enemy, spawnPath.GlobalPosition);
	}

	private void LoadPatternPool() // loads patterns through the filepath below into the patternPool with their associated spawningCost
	{
		//commented out version only works for desktop and not for mobile devices
		/*const string patternFilepath = "res://Utilities/Gameflow/Spawn/Patterns/";
		foreach (var patternName in DirAccess.GetFilesAt(patternFilepath))
		{
			PackedScene pattern = GD.Load<PackedScene>(patternFilepath + patternName);
			_patternPool.Add(pattern, pattern.Instantiate<EnemyPattern>().spawningCost);
		}*/

		var enemy_patterns = FileAccess.Open("res://Utilities/Gameflow/Spawn/Patterns/enemy_patterns.json", FileAccess.ModeFlags.Read);
		if (enemy_patterns == null)
		{
			Debug.Print("Could not open enemy_patterns.json");
			return;
		}
		var content = enemy_patterns.GetAsText();
		var patternPaths = Json.ParseString(content).AsGodotArray<string>();

		foreach (var path in patternPaths)
		{
			var pattern = GD.Load<PackedScene>(path);
			if (pattern != null)
			{
				var instance = pattern.Instantiate<EnemyPattern>();
				_patternPool[pattern] = instance.spawningCost;
			}
		}
	}
	private void OnWaveStart() 
	{
		_healthMultiplier = 1f + (_currentWave - 1) * (0.1f + (_playerCount-1)*0.05f);
		_damageMultiplier = 1f + (_currentWave - 1) * (0.2f + (_playerCount-1)*0.1f);
		_attackSpeedMultiplier = Math.Min(1f + (_currentWave - 1) * 0.05f , 2f);
		_moveSpeedMultiplier = Math.Min(1f + (_currentWave - 1) * 0.0375f , 1.5f);

		if (_currentWave % 5 == 2) // the giant will spawn in wave 2 (Note: the normal enemies also spawn)
		{
			for (int i = 0; i < 1 + (_currentWave / 5); i++)
			{
				spawnGiantBoss();
			}
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

		var spawnValue = GD.Randf() * Math.Min(_currentWave,3); // generate a random spawnValue to determine the difficulty of the selected enemies, capped at 3 so a larger variety of enemies can spawn during later waves

		var patternPoolCopy = _patternPool; // copy of patternPool so the loaded patterns don't get removed
		patternPoolCopy = patternPoolCopy.
			Where(i => i.Key.Instantiate<EnemyPattern>().minWave <= _currentWave && (i.Key.Instantiate<EnemyPattern>().maxWave >= _currentWave | i.Key.Instantiate<EnemyPattern>().maxWave == -1)). // look for patterns in the correct wave number range. max -1 can allways spawn
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
