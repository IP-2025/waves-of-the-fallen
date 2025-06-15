namespace Game.Utilities.Multiplayer;

using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System;
using System.Threading;
using System.Threading.Tasks;
using Game.UI.GameOver;
using Game.Utilities.Backend;

public partial class Client : Node
{
	private bool _enableDebug = true;
	private Camera2D _camera;
	private bool _hasJoystick;
	private bool _waveTimerReady;
	private WaveTimer _timer;
	private bool _graceTimeTriggered;

	// movement tracking of players
	private Dictionary<long, Vector2> _lastPositions = new();

	// GameRoot container for entities
	private readonly Dictionary<long, Node2D> _instances = new();

	// Shop
	private int _lastLocalShopRound = 1;
	private int _newWeaponPos = 0;
	private PackedScene _shopScene = GD.Load<PackedScene>("res://UI/Shop/BossShop/bossShop.tscn");
	private Node _shopInstance;
	private bool weaponUpdated = false;
	private string _selectedWeapon = "";

	// mapping per entity type
	private readonly Dictionary<EntityType, PackedScene> _prefabs = new()
	{
		{ EntityType.DefaultPlayer, GD.Load<PackedScene>("res://Entities/Characters/Base/default_player.tscn") },
		{ EntityType.Archer, GD.Load<PackedScene>("res://Entities/Characters/Archer/archer.tscn") },
		{ EntityType.DefaultEnemy,  GD.Load<PackedScene>("res://Entities/Enemies/Goblin/default_enemy.tscn") },
		{ EntityType.RangedEnemy,  GD.Load<PackedScene>("res://Entities/Enemies/Skeleton/ranged_enemy.tscn") },
		{ EntityType.MountedEnemy,  GD.Load<PackedScene>("res://Entities/Enemies/Rider/mounted_enemy.tscn") },
		{ EntityType.RiderEnemy,  GD.Load<PackedScene>("res://Entities/Enemies/Rider/rider_enemy.tscn") },
		{ EntityType.GiantBossEnemy,  GD.Load<PackedScene>("res://Entities/Enemies/GiantBoss/giantBossEnemy.tscn") },
		{ EntityType.Bow,  GD.Load<PackedScene>("res://Weapons/Ranged/Bow/bow.tscn") },
		{ EntityType.BowArrow,  GD.Load<PackedScene>("res://Weapons/Ranged/Bow/bow_arrow.tscn") },
		{ EntityType.Crossbow,  GD.Load<PackedScene>("res://Weapons/Ranged/Crossbow/crossbow.tscn") },
		{ EntityType.CrossbowArrow,  GD.Load<PackedScene>("res://Weapons/Ranged/Crossbow/crossbow_arrow.tscn") },
		{ EntityType.Kunai, GD.Load<PackedScene>("res://Weapons/Ranged/Kunai/kunai.tscn") },
		{ EntityType.KunaiProjectile, GD.Load<PackedScene>("res://Weapons/Ranged/Kunai/kunai_projectile.tscn")},
		{ EntityType.Mage, GD.Load<PackedScene>("res://Entities/Characters/Mage/mage.tscn") },
		{ EntityType.Knight, GD.Load<PackedScene>("res://Entities/Characters/Knight/knight.tscn") },
		{ EntityType.Assassin, GD.Load<PackedScene>("res://Entities/Characters/Assassin/assassin.tscn") },
		{ EntityType.FireStaff, GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Firestaff/firestaff.tscn")},
		{ EntityType.FireBall, GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Firestaff/fireball.tscn")},
		{ EntityType.Lightningstaff, GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Lightningstaff/lightningstaff.tscn")},
		{ EntityType.Lighting, GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Lightningstaff/lightning.tscn")},
		{ EntityType.Dagger, GD.Load<PackedScene>("res://Weapons/Melee/Dagger/dagger.tscn")},
		{ EntityType.Sword, GD.Load<PackedScene>("res://Weapons/Melee/MasterSword/Sword.tscn")},
		{ EntityType.WarHammer, GD.Load<PackedScene>("res://Weapons/Ranged/WarHammer/warHammer.tscn")},
		{ EntityType.HammerProjectile, GD.Load<PackedScene>("res://Weapons/Ranged/WarHammer/hammerProjectile.tscn")},
		{ EntityType.HealStaff, GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Healsftaff/healstaff.tscn")},
		{ EntityType.DoubleBlade, GD.Load<PackedScene>("res://Weapons/Melee/DoubleBlades/DoubleBlade.tscn")},
		{ EntityType.MedicineBag, GD.Load<PackedScene>("res://Weapons/Utility/MedicineBag/medicineBag.tscn")},
		{ EntityType.Medicine, GD.Load<PackedScene>("res://Weapons/Utility/MedicineBag/medicine.tscn")}
	};

	public override void _Ready()
	{
	}

	public Command GetCommand(ulong tick)
	{
		long eid = Multiplayer.GetUniqueId();

		var joy = GetLocalJoystickDirection();
		var key = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		// decide between joystick and keyboard input, joystick has priority
		var dir = joy != Vector2.Zero ? joy : key;

		// nonly send move command if there is input
		return dir != Vector2.Zero ? new Command(tick, eid, CommandType.Move, dir, _selectedWeapon, _newWeaponPos) : null;
	}

	public Command GetShopCommand(ulong tick)
	{
		long eid = Multiplayer.GetUniqueId();

		var joy = GetLocalJoystickDirection();
		var key = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		// decide between joystick and keyboard input, joystick has priority
		var dir = joy != Vector2.Zero ? joy : key;

		bool executeCommand = weaponUpdated;
		weaponUpdated = false;

		return executeCommand ? new Command(tick, eid, CommandType.BossShop, dir, _selectedWeapon, _newWeaponPos) : null;
	}

	// helper finds the local player's joystick and returns its direction
	private Vector2 GetLocalJoystickDirection()
	{
		// scene root - GameRoot
		var playerNodeName = $"E_{Multiplayer.GetUniqueId()}";
		var playerNode = GetTree()
			.Root.GetNodeOrNull<GameRoot>("GameRoot")
			?.GetNodeOrNull<CharacterBody2D>(playerNodeName);

		var joystick = playerNode?.GetNodeOrNull<Joystick>("Joystick");

		if (joystick == null)
			return Vector2.Zero;

		DebugIt($"Joystick direction: {joystick.PosVector}");
		return joystick.PosVector;
	}

	public void ApplySnapshot(Snapshot snap)
	{
		// check if all players are dead in snapshot
		ShowGameOverScreen(snap.livingPlayersCount);

		// collect all network ids from the snapshot
		var networkIds = snap.Entities.Select(e => e.NetworkId).ToHashSet();

		// Update PlayerScores from the snapshot
		UpdatePlayerScores(snap.Entities);

		// instanciate or update entities
		InstantiateOrUpdateEntities(snap.Entities);

		// cleanup removed entities
		CleanupRemovedEntities(networkIds);

		if (_camera == null || networkIds.Contains(Multiplayer.GetUniqueId())) return;
		_camera = null;
		_hasJoystick = false;
	}

	private void UpdatePlayerScores(IEnumerable<EntitySnapshot> entities)
	{
		foreach (var entity in entities)
		{
			if (entity.PlayerScores != null)
			{
				foreach (var score in entity.PlayerScores)
				{
					// update the ScoreManager with the scores from the snapshot
					if (!ScoreManager.PlayerScores.ContainsKey(score.Key))
					{
						ScoreManager.PlayerScores[score.Key] = 0;
					}
					ScoreManager.PlayerScores[score.Key] = score.Value;
				}
			}
		}
	}

	private void ShowGameOverScreen(int livingPlayersCount)
	{
		if (livingPlayersCount == 0)
		{
			DebugIt("Show Game Over screen, no one is alive");
			var gameRoot = GetTree().Root.GetNodeOrNull<GameRoot>("GameRoot");
			gameRoot.ShowGameOverScreen();
		}
	}

	private void OnWeaponChosen(Weapon weaponType)
	{
		if (_shopInstance != null && IsInstanceValid(_shopInstance))
			_shopInstance.QueueFree();
		_shopInstance = null;

		_selectedWeapon = weaponType.GetType().Name;
		_newWeaponPos++;
		weaponUpdated = true;
	}

	private void InstantiateOrUpdateEntities(IEnumerable<EntitySnapshot> entities)
	{
		// Kamera- und WaveTimer-Referenzen überprüfen und ggf. zurücksetzen
		if (_camera != null && !IsInstanceValid(_camera))
		{
			_camera = null;
			_waveTimerReady = false;
			_timer = null;
		}
		// first all not a weapon things (no OwnerID & SlotIndex)
		foreach (var entity in entities.Where(e => !e.OwnerId.HasValue || !e.SlotIndex.HasValue))
		{

			// Shop
			if (entity.WaveCount > _lastLocalShopRound && entity.WaveCount < 5)
			{
				_lastLocalShopRound = entity.WaveCount;

				if (_camera != null && _shopInstance == null)
				{
					_shopInstance = _shopScene.Instantiate();
					_shopInstance.Connect(nameof(BossShop.WeaponChosen), new Callable(this, nameof(OnWeaponChosen)));
					_camera.AddChild(_shopInstance);

					DebugIt($"Shop instantiated for wave {entity.WaveCount}");
				}
			}

			switch (_waveTimerReady)
			{
				// HUD / WaveCounter stuff
				case false when _camera != null:
					{
						var wt = GD.Load<PackedScene>("res://Utilities/Gameflow/Waves/WaveTimer.tscn").Instantiate<WaveTimer>();
						wt.Disable = true;
						_camera.AddChild(wt);
						_timer = wt;
						_waveTimerReady = true;
						break;
					}
				case true when _camera != null:
					{
						var timeLeftLabel = _timer.GetNodeOrNull<Label>("TimeLeft");
						if (timeLeftLabel != null)
							timeLeftLabel.Text = (_timer.MaxTime - entity.WaveTimeLeft).ToString();

						var waveCounterLabel = _timer.GetNodeOrNull<Label>("WaveCounter");
						if (waveCounterLabel != null)
							waveCounterLabel.Text = $"Wave: {entity.WaveCount}";
						if (entity.GraceTime)
						{
							timeLeftLabel.Text = "Grace Time";
							if (!_graceTimeTriggered)
							{
								_timer.TriggerWaveEnded();
								_graceTimeTriggered = true;
							}
						}
						else
						{
							_graceTimeTriggered = false; // Reset, wenn GraceTime vorbei ist
						}

						break;
					}
			}

			// Player stuff
			if (!_instances.TryGetValue(entity.NetworkId, out var inst))
			{
				inst = CreateInstance(entity);
				if (inst == null)
					continue;

				_instances[entity.NetworkId] = inst;
				DebugIt($"Instantiated {entity.Type} with ID {entity.NetworkId}");

				if (!_hasJoystick && entity.NetworkId == Multiplayer.GetUniqueId())
				{
					AttachJoystick(inst, entity);
				}

				if (_camera == null && entity.NetworkId == Multiplayer.GetUniqueId())
				{
					ChangeCamera(inst, entity);
				}

				// check HUD for local player
				if (entity.NetworkId == Multiplayer.GetUniqueId())
				{
					if (GetTree().Root.GetNodeOrNull("HUD") == null)
					{
						var hudScene = GD.Load<PackedScene>("res://UI/HUD/HUD.tscn");
						var hud = hudScene.Instantiate();
						hud.Name = "HUD";
						GetTree().Root.AddChild(hud);
					}
				}
			}

			UpdateTransform(inst, entity);
		}

		// Weapon Handling
		foreach (var entity in entities.Where(e => e.OwnerId.HasValue && e.SlotIndex.HasValue))
		{
			if (_instances.ContainsKey(entity.NetworkId))
				continue;

			var inst = CreateInstance(entity);
			if (inst == null)
				continue; // cant find owner / slot

			_instances[entity.NetworkId] = inst;
			DebugIt($"Instantiated weapon {entity.Type} with ID {entity.NetworkId} under owner {entity.OwnerId.Value}");
		}

		// guarantee that the PauseMenu for the local player ALWAYS exists
		var hudNode = GetTree().Root.GetNodeOrNull<CanvasLayer>("HUD");
		if (hudNode != null && hudNode.GetNodeOrNull<PauseMenu>("PauseMenu") == null)
		{
			var pauseMenuScene = GD.Load<PackedScene>("res://Menu/PauseMenu/pauseMenu.tscn");
			var pauseMenu = pauseMenuScene.Instantiate<PauseMenu>();
			pauseMenu.Name = "PauseMenu";
			hudNode.AddChild(pauseMenu);
			pauseMenu.Visible = false;
		}
	}

	private Node2D CreateInstance(EntitySnapshot entity)
	{
		if (!entity.OwnerId.HasValue)
		{
			if (!_prefabs.TryGetValue(entity.Type, out var scene))
			{
				GD.PrintErr($"Cant find path for {entity.Type}");
				return null;
			}

			var inst = scene.Instantiate<Node2D>();

			// Set OwnerPeerId for player instances
			if (inst is DefaultPlayer dp)
			{
				dp.OwnerPeerId = entity.NetworkId;

				var healthNode = inst.GetNodeOrNull<Health>("Health");
				if (healthNode != null)
				{
					healthNode.MaxHealth = entity.Health; // <- MaxHealth aus dem Snapshot setzen!
					healthNode.max_health = entity.Health;
					healthNode.health = entity.Health;
					healthNode.ResetHealth();
				}
			}

			// disable health for enemies / player because server handles it
			if (entity.Type == EntityType.DefaultEnemy
				|| entity.Type == EntityType.RangedEnemy
				|| entity.Type == EntityType.MountedEnemy
				|| entity.Type == EntityType.RiderEnemy
				|| entity.Type == EntityType.GiantBossEnemy
				|| entity.Type == EntityType.DefaultPlayer
				|| entity.Type == EntityType.Archer
				|| entity.Type == EntityType.Assassin
				|| entity.Type == EntityType.Knight
				|| entity.Type == EntityType.Mage)
			{
				var healthNode = inst.GetNodeOrNull<Health>("Health");
				healthNode.disable = true;
				healthNode.health = entity.Health * 100; // high value so that client cant kill and cant be killed. Server handles it
			}

			if (entity.Type == EntityType.DefaultEnemy
				|| entity.Type == EntityType.RangedEnemy
				|| entity.Type == EntityType.MountedEnemy
				|| entity.Type == EntityType.RiderEnemy
				|| entity.Type == EntityType.GiantBossEnemy)
			{
				inst.AddToGroup("enemies");
			}


			inst.Name = $"E_{entity.NetworkId}";
			inst.Scale = entity.Scale;
			GetNode<GameRoot>("/root/GameRoot").AddChild(inst);
			return inst;
		}

		// weapons
		// if it has OwnerId and SlotIndex, it is a weapon
		if (entity.OwnerId.HasValue && entity.SlotIndex.HasValue)
		{
			if (!_prefabs.TryGetValue(entity.Type, out var scene))
			{
				GD.PrintErr($"Cant find weapon path for {entity.Type}");
				return null;
			}

			var inst = scene.Instantiate<Node2D>();
			inst.Name = $"E_{entity.NetworkId}";
			inst.Position = Vector2.Zero;

			// find owner node
			var ownerNode = GetTree()
				.Root
				.GetNode<GameRoot>("GameRoot")
				.GetNode<Node2D>($"E_{entity.OwnerId.Value}");

			if (ownerNode == null)
			{
				GD.PrintErr($"Owner E_{entity.OwnerId.Value} cant be found, waiting for next snapshot");
				return null;
			}
			// slot container / slots for weapons
			var slots = ownerNode.GetNodeOrNull<Node2D>("WeaponSpawnPoints");
			if (slots == null)
			{
				GD.PrintErr($"Cant find WeaponSpawnPoints {ownerNode.Name}");
				return null;
			}

			int idx = entity.SlotIndex.Value;
			if (idx < 0 || idx >= slots.GetChildCount())
			{
				GD.PrintErr($"Invalid SlotIndex {idx} on {ownerNode.Name}");
				return null;
			}
			var slot = slots.GetChild<Node2D>(idx);
			// Remove all existing weapons from the slot before adding the new weapon instance
			for (int i = slot.GetChildCount() - 1; i >= 0; i--)
			{
				var child = slot.GetChild(i);
				if (IsInstanceValid(child))
					child.QueueFree();
			}
			slot.AddChild(inst);
			return inst;
		}

		return null; // no OwnerID & SlotIndex? f this
	}




	private void UpdateTransform(Node2D inst, EntitySnapshot entity)
	{
		if (IsInstanceValid(inst))
		{
			Vector2 lastPos = _lastPositions.TryGetValue(entity.NetworkId, out var lp) ? lp : entity.Position;
			Vector2 deltaPos = entity.Position - lastPos;

			inst.GlobalPosition = entity.Position;
			inst.Rotation = entity.Rotation;
			inst.Scale = entity.Scale;
			inst.GetNodeOrNull<Health>("Health").health = entity.Health;

			// Sync all animations for players
			if (inst is DefaultPlayer player)
			{
				if (entity.Health <= 0)
				{
					player.animationHandler?.SetDeath();
				}
				else
				{
					// Flip based on movement direction
					if (player.animation != null && Math.Abs(deltaPos.X) > 1e-2)
						player.animation.FlipH = deltaPos.X < 0;

					// Walk/Idle Animation based on movement
					if (player.animationHandler != null)
					{
						if (deltaPos.Length() > 1e-2)
							player.animationHandler.UpdateAnimationState(false, deltaPos);
						else
							player.animationHandler.UpdateAnimationState(false, Vector2.Zero);
					}
				}
			}

			// save last position
			_lastPositions[entity.NetworkId] = entity.Position;
		}
	}

	private void AttachJoystick(Node2D inst, EntitySnapshot entity)
	{
		// only for local / this clients player
		bool isPlayerType = entity.Type is EntityType.DefaultPlayer or EntityType.Archer
							|| entity.Type == EntityType.Knight
							|| entity.Type == EntityType.Mage
							|| entity.Type == EntityType.Assassin;
		if (!isPlayerType || entity.NetworkId != Multiplayer.GetUniqueId())
		{
			return;
		}

		var joystick = GD.Load<PackedScene>("res://UI/Joystick/joystick.tscn").Instantiate<Node2D>();
		inst.AddChild(joystick);
		DebugIt($"Joystick added to player with ID {entity.NetworkId}");
	}

	private void ChangeCamera(Node2D inst, EntitySnapshot entity)
	{
		bool isPlayerType = entity.Type == EntityType.DefaultPlayer
							|| entity.Type == EntityType.Archer
							|| entity.Type == EntityType.Knight
							|| entity.Type == EntityType.Mage
							|| entity.Type == EntityType.Assassin;
		// only for local / this clients player
		if (!isPlayerType || entity.NetworkId != Multiplayer.GetUniqueId())
			return;

		if (entity.NetworkId == Multiplayer.GetUniqueId())
		{
			_camera = inst.GetNodeOrNull<Camera2D>("Camera2D");
			_camera.MakeCurrent();
			DebugIt($"Camera set to player with ID {entity.NetworkId}");
		}
	}

	private void CleanupRemovedEntities(HashSet<long> validIds)
	{
		var toRemove = _instances.Keys.Where(id => !validIds.Contains(id)).ToList();
		foreach (var id in toRemove)
		{
			var inst = _instances[id];
			if (inst != null && GodotObject.IsInstanceValid(inst))
				inst.QueueFree();
			_instances.Remove(id);
			DebugIt($"Removed entity with ID {id}");
		}
	}

	private void DebugIt(string message)
	{
		if (_enableDebug) Debug.Print("Client: " + message);
	}
}
