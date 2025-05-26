namespace Game.Utilities.Multiplayer
{
	
using Godot;
using System.Collections.Generic;
using System.Reflection.Metadata;

public partial class Server : Node
{
	public static Server Instance;

	private bool enableDebug = false;
	public Dictionary<long, int> PlayerSelections = new Dictionary<long, int>();
	public Dictionary<long, Node2D> Entities = new Dictionary<long, Node2D>();
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
			.Root.GetNodeOrNull<GameRoot>("GameRoot")
			?.GetNodeOrNull<WaveTimer>("GlobalWaveTimer");

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
				slotIx
			));
		}

		foreach (var id in toRemove)
		{
			Entities.Remove(id);
		}

		return Serializer.Serialize(snap);
	}

	private void DebugIt(string message)
	{
		if (enableDebug)
			GD.Print($"Server: {message}");
	}
}	
}
