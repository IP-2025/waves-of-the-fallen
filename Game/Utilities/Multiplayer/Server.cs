namespace Game.Utilities.Multiplayer
{
	using Game.Utilities.Backend;
	using Godot;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Reflection.Metadata;

	public partial class Server : Node
	{
		public static Server Instance;

		private bool enableDebug = false;
		public Dictionary<long, PlayerCharacterData> PlayerSelections = new Dictionary<long, PlayerCharacterData>();
		public Dictionary<long, Node2D> Entities = new Dictionary<long, Node2D>();

		private PackedScene _bowScene = GD.Load<PackedScene>("res://Weapons/Ranged/Bow/bow.tscn");
		private PackedScene _crossbowScene = GD.Load<PackedScene>("res://Weapons/Ranged/Crossbow/crossbow.tscn");
		private PackedScene _kunaiScene = GD.Load<PackedScene>("res://Weapons/Ranged/Kunai/kunai.tscn");
		private PackedScene _fireStaffScene = GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Firestaff/firestaff.tscn");
		private PackedScene _lightningStaffScene = GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Lightningstaff/lightningstaff.tscn");
		private PackedScene _daggerScene = GD.Load<PackedScene>("res://Weapons/Melee/Dagger/dagger.tscn");
		private PackedScene _swordScene = GD.Load<PackedScene>("res://Weapons/Melee/MasterSword/Sword.tscn");
		private PackedScene _warHammerScene = GD.Load<PackedScene>("res://Weapons/Ranged/WarHammer/warHammer.tscn");
		private PackedScene _medicineBagScene = GD.Load<PackedScene>("res://Weapons/Utility/MedicineBag/medicineBag.tscn");
		private PackedScene _healStaffScene = GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Healsftaff/healstaff.tscn");
		private PackedScene _doubleBladeScene = GD.Load<PackedScene>("res://Weapons/Melee/DoubleBlades/DoubleBlade.tscn");

		private static readonly Dictionary<string, EntityType> ScenePathToEntityType = new()
	{
		{ "res://Entities/Characters/Base/default_player.tscn", EntityType.DefaultPlayer },
		{ "res://Entities/Characters/Archer/archer.tscn", EntityType.Archer },
		{ "res://Entities/Enemies/Goblin/default_enemy.tscn", EntityType.DefaultEnemy },
		{ "res://Entities/Enemies/Rider/mounted_enemy.tscn", EntityType.MountedEnemy },
		{ "res://Entities/Enemies/Skeleton/ranged_enemy.tscn", EntityType.RangedEnemy },
		{ "res://Entities/Enemies/Rider/rider_enemy.tscn", EntityType.RiderEnemy },
		{ "res://Entities/Enemies/GiantBoss/giantBossEnemy.tscn", EntityType.GiantBossEnemy },
		{ "res://Weapons/Ranged/Bow/bow.tscn", EntityType.Bow },
		{ "res://Weapons/Ranged/Bow/bow_arrow.tscn", EntityType.BowArrow },
		{ "res://Weapons/Ranged/Crossbow/crossbow.tscn", EntityType.Crossbow },
		{ "res://Weapons/Ranged/Crossbow/crossbow_arrow.tscn", EntityType.CrossbowArrow },
		{ "res://Weapons/Ranged/Kunai/kunai.tscn", EntityType.Kunai },
		{ "res://Weapons/Ranged/Kunai/kunai_projectile.tscn", EntityType.KunaiProjectile },
		{ "res://Entities/Characters/Knight/knight.tscn", EntityType.Knight },
		{ "res://Entities/Characters/Assassin/assassin.tscn", EntityType.Assassin },
		{ "res://Entities/Characters/Mage/mage.tscn", EntityType.Mage },
		{ "res://Weapons/Ranged/MagicStaffs/Firestaff/firestaff.tscn", EntityType.FireStaff },
		{ "res://Weapons/Ranged/MagicStaffs/Firestaff/fireball.tscn", EntityType.FireBall },
		{ "res://Weapons/Melee/Dagger/dagger.tscn", EntityType.Dagger },
		{ "res://Weapons/Melee/MasterSword/Sword.tscn", EntityType.Sword },
		{ "res://Weapons/Ranged/MagicStaffs/Lightningstaff/lightningstaff.tscn", EntityType.Lightningstaff },
		{ "res://Weapons/Ranged/MagicStaffs/Lightningstaff/lightning.tscn", EntityType.Lighting },
		{ "res://Weapons/Ranged/WarHammer/warHammer.tscn", EntityType.WarHammer },
		{ "res://Weapons/Ranged/WarHammer/hammerProjectile.tscn", EntityType.HammerProjectile },
		{ "res://Weapons/Ranged/MagicStaffs/Healsftaff/healstaff.tscn", EntityType.HealStaff },
		{ "res://Weapons/Melee/DoubleBlades/DoubleBlade.tscn", EntityType.DoubleBlade },
		{ "res://Weapons/Utility/MedicineBag/medicineBag.tscn", EntityType.MedicineBag },
		{ "res://Weapons/Utility/MedicineBag/medicine.tscn", EntityType.Medicine },
		{ "res://UI/Ability/Ablities/boost_dexterity.tscn", EntityType.BoostDexterity},
		{ "res://UI/Ability/Ablities/boost_strength.tscn", EntityType.BoostStrength},
		{ "res://UI/Ability/Ablities/boost_intelligence.tscn", EntityType.BoostIntelligence},
		{ "res://UI/Ability/Ablities/speed_up.tscn", EntityType.SpeedUp},
		{ "res://UI/Ability/Ablities/arrow_rain.tscn", EntityType.ArrowRain},
		{ "res://UI/Ability/Ablities/shield.tscn", EntityType.Shield},
		{ "res://UI/Ability/Ablities/fire_blast.tscn", EntityType.FireBlast},
		{ "res://UI/Ability/Ablities/deadly_strike.tscn", EntityType.DeadlyStrike}
	};

		public override void _Ready()
		{
			Instance = this;
		}

		public void ProcessCommand(Command cmd)
		{
			if (!Entities.TryGetValue(cmd.EntityId, out var entity))
			{
				DebugIt($"Entity {cmd.EntityId} not found in Entities dictionary");
				return;
			}

			if (cmd.Type == CommandType.Move && cmd.MoveDir.HasValue)
			{
				var dir = cmd.MoveDir.Value;
				if (dir.Length() > 1f)
				{
					dir = dir.Normalized();
				}

				var joystick = entity.GetNodeOrNull<Joystick>("Joystick");
				if (joystick != null)
				{
					joystick.PosVector = dir;
					DebugIt($"Set Joystick.PosVector = {dir} on EntityID {cmd.EntityId}");
				}
			}
			else if (cmd.Type == CommandType.Shoot)
			{
				// Maybe we need, maybe we don't
			}
			else if (cmd.Type == CommandType.BossShop)
			{
				var scene = cmd.Weapon switch
				{
					"Bow" => _bowScene,
					"Crossbow" => _crossbowScene,
					"FireStaff" => _fireStaffScene,
					"Kunai" => _kunaiScene,
					"Lightningstaff" => _lightningStaffScene,
					"Healstaff" => _healStaffScene,
					"Dagger" => _daggerScene,
					"Sword" => _swordScene,
					"WarHammer" => _warHammerScene,
					"DoubleBlade" => _doubleBladeScene,
					_ => null
				};
				if (scene == null) return;
				var slot = entity.GetNode("WeaponSpawnPoints").GetChild(cmd.WeaponPos) as Node2D;
				var weapon = scene.Instantiate<Area2D>();
				slot?.AddChild(weapon);
				weapon.Position = Vector2.Up;

				var id = weapon.GetInstanceId();
				weapon.Name = $"Weapon_{id}";
				weapon.SetMeta("OwnerId", cmd.EntityId);
				weapon.SetMeta("SlotIndex", cmd.WeaponPos);
				Instance.Entities.Add((long)id, weapon);
			}
			else if (cmd.Type == CommandType.Ability)
			{
				Debug.Print("ABILITY BEI SERVER ANGEKOMMEN");
				/*
				var scene = cmd.Weapon switch
				{
					"Bow" => _bowScene,
					"Crossbow" => _crossbowScene,
					"FireStaff" => _fireStaffScene,
					"Kunai" => _kunaiScene,
					"Lightningstaff" => _lightningStaffScene,
					"Healstaff" => _healStaffScene,
					"Dagger" => _daggerScene,
					"Sword" => _swordScene,
					"WarHammer" => _warHammerScene,
					"DoubleBlade" => _doubleBladeScene,
					_ => null
				};
				if (scene == null) return;*/
				if (entity is DefaultPlayer defaultPlayer)
				{
					defaultPlayer.UseAbility();
				}
			}
		}

		public byte[] GetSnapshot(ulong tick)
		{
			var snap = new Snapshot(tick);
			var toRemove = new List<long>();
			foreach (var kv in Entities)
			{
				var node = kv.Value;

				if (node == null || !IsInstanceValid(node))
				{
					toRemove.Add(kv.Key);
					continue;
				}

				string scenePath = node.SceneFilePath;
				var id = kv.Key;

				if (!ScenePathToEntityType.TryGetValue(scenePath, out var entityType))
				{
					GD.PrintErr($"Unknown ScenePath: {scenePath}");
					continue;
				}

				// Find WaveTimer as a child of the current camera
				int waveCount = 0;
				int secondsLeft = 0;
				bool graceTime = false;
				var waveTimer = GetTree()
					.Root
					.GetNodeOrNull<GameRoot>("GameRoot")
					.GetNodeOrNull<WaveTimer>("GlobalWaveTimer");

				if (waveTimer != null)
				{
					waveCount = waveTimer.WaveCounter;
					secondsLeft = waveTimer.SecondCounter;
					graceTime = waveTimer.IsPaused;
				}

				var healthNode = node.GetNodeOrNull<Health>("Health");
				float health = healthNode != null ? healthNode.health : 0f;

				long? owner = null;
				int? slotIx = null;
				if (node.HasMeta("OwnerId"))
				{
					owner = (long)node.GetMeta("OwnerId");
					slotIx = (int)node.GetMeta("SlotIndex");
				}

				DebugIt($"Snapshot: Entity Name: {node.Name}, Position: {node.Position}, ID: {id}");
				snap.Entities.Add(new EntitySnapshot(
					id,
					node.Position,
					node.Rotation,
					node.Scale,
					health,
					entityType,
					waveCount,
					secondsLeft,
					graceTime,
					owner,
					slotIx,
					ScoreManager.PlayerScores
				));
			}

			foreach (var id in toRemove)
			{
				Entities.Remove(id);
			}

			int livingPlayers = Entities
									.Values
									.OfType<DefaultPlayer>() // oder dein Basistyp für Spieler
									.Count(player =>
										{
											var health = player.GetNodeOrNull<Health>("Health");
											return health != null && health.health > 0;
										});

			snap.livingPlayersCount = livingPlayers;

			return Serializer.Serialize(snap);
		}

		public void syncHostWaveTimer()
		{
			// Get hosts WaveTimer (attached to Player_1 Camera2D)
			WaveTimer loacalWT = GetTree()
				.Root
				.GetNodeOrNull<GameRoot>("GameRoot")?
				.GetNodeOrNull<DefaultPlayer>("Player_1")?
				.GetNodeOrNull<Camera2D>("Camera2D")?
				.GetNodeOrNull<WaveTimer>("WaveTimer");
			if (loacalWT == null) return;

			// Get global WaveTimer
			WaveTimer globalWT = GetTree()
				.Root
				.GetNodeOrNull<GameRoot>("GameRoot")
				.GetNodeOrNull<WaveTimer>("GlobalWaveTimer");
			if (globalWT == null) return;

			var gTimeLeftLabel = globalWT.GetNodeOrNull<Label>("TimeLeft");
			var lTimeLeftLabel = loacalWT.GetNodeOrNull<Label>("TimeLeft");
			lTimeLeftLabel.Text = gTimeLeftLabel.Text;

			var gWaveCounterLabel = globalWT.GetNodeOrNull<Label>("WaveCounter");
			var lWaveCounterLabel = loacalWT.GetNodeOrNull<Label>("WaveCounter");
			lWaveCounterLabel.Text = gWaveCounterLabel.Text;
		}

		private void DebugIt(string message)
		{
			if (enableDebug)
				GD.Print($"Server: {message}");
		}

		[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
		public void PlayerLeft(long playerId)
		{
			// remove entity from Entities dictionary if player left
			if (Entities.TryGetValue(playerId, out var node))
			{
				if (IsInstanceValid(node))
					node.QueueFree();
				Entities.Remove(playerId);
				GD.Print($"Player {playerId} has left the game and was removed.");
			}
			PlayerSelections.Remove(playerId);
		}
	}
}
