using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;

public partial class Client : Node
{
	bool enableDebug = false;
	private Camera2D _camera;
	private bool _hasJoystick = false;
	private bool _waveTimerReady = false;
	private WaveTimer timer = null;

	// GameRoot container for entities
	private Dictionary<long, Node2D> _instances = new();

    // mapping per entity type
    private Dictionary<EntityType, PackedScene> _prefabs = new()
    {
        { EntityType.DefaultPlayer, GD.Load<PackedScene>("res://Entities/Characters/Base/default_player.tscn") },
        { EntityType.Archer, GD.Load<PackedScene>("res://Entities/Characters/Archer/archer.tscn") },
        { EntityType.DefaultEnemy,  GD.Load<PackedScene>("res://Entities/Enemies/Goblin/default_enemy.tscn") },
        { EntityType.RangedEnemy,  GD.Load<PackedScene>("res://Entities/Enemies/Skeleton/ranged_enemy.tscn") },
        { EntityType.MountedEnemy,  GD.Load<PackedScene>("res://Entities/Enemies/Rider/mounted_enemy.tscn") },
        { EntityType.RiderEnemy,  GD.Load<PackedScene>("res://Entities/Enemies/Rider/rider_enemy.tscn") },
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
		{ EntityType.FireBall, GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Firestaff/fireball.tscn")}
    };

	public override void _Ready()
	{
	}

	public Command GetCommand(ulong tick)
	{
		long eid = Multiplayer.GetUniqueId();

		Vector2 joy = GetLocalJoystickDirection();
		var key = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		// decide between joystick and keyboard input, joystick has priority
		var dir = joy != Vector2.Zero ? joy : key;

		// nonly send move command if there is input
		if (dir != Vector2.Zero)
		{
			return new Command(tick, eid, CommandType.Move, dir);
		}

		return null;
	}

	// helper finds the local player's joystick and returns its direction
	private Vector2 GetLocalJoystickDirection()
	{
		// scene root - GameRoot
		string playerNodeName = $"E_{Multiplayer.GetUniqueId()}";
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
		// collect all network ids from the snapshot
		var networkIds = snap.Entities.Select(e => e.NetworkId).ToHashSet();

		// instanciate or update entities
		InstantiateOrUpdateEntities(snap.Entities);

		// cleanup removed entities
		CleanupRemovedEntities(networkIds);
	}

    private void InstantiateOrUpdateEntities(IEnumerable<EntitySnapshot> entities)
    {
        // first all not a weapon things (no OwnerID & SlotIndex)
        foreach (var entity in entities.Where(e => !e.OwnerId.HasValue || !e.SlotIndex.HasValue))
        {
            // HUD / WaveCounter stuff
            if (!_waveTimerReady && _camera != null)
            {
                var wt = GD.Load<PackedScene>("res://Utilities/Gameflow/Waves/WaveTimer.tscn").Instantiate<WaveTimer>();
                wt.disable = true;
                _camera.AddChild(wt);
                timer = wt;
                _waveTimerReady = true;
            }
            else if (_waveTimerReady && _camera != null)
            {
                var timeLeftLabel = timer.GetNodeOrNull<Label>("TimeLeft");
                if (timeLeftLabel != null)
                    timeLeftLabel.Text = (timer.maxTime - entity.WaveTimeLeft).ToString();

				var waveCounterLabel = timer.GetNodeOrNull<Label>("WaveCounter");
				if (waveCounterLabel != null)
					waveCounterLabel.Text = $"Wave: {entity.WaveCount}";
				if (entity.GraceTime) timeLeftLabel.Text = $"Grace Time";
			}

			// Player stuff
			if (!_instances.TryGetValue(entity.NetworkId, out var inst))
			{
				inst = CreateInstance(entity);
				if (inst == null)
					continue;

				_instances[entity.NetworkId] = inst;
				DebugIt($"Instantiated {entity.Type} with ID {entity.NetworkId}");

				if (!_hasJoystick)
				{
					AttachJoystick(inst, entity);
				}

				if (_camera == null)
				{
					ChangeCamera(inst, entity);
				}
			}

			UpdateTransform(inst, entity);
		}

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

            // sisable health for enemies because server handles it
            if (entity.Type == EntityType.DefaultEnemy
                || entity.Type == EntityType.RangedEnemy
                || entity.Type == EntityType.MountedEnemy
                || entity.Type == EntityType.RiderEnemy
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
				|| entity.Type == EntityType.RiderEnemy)
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
			slot.AddChild(inst);
			return inst;
		}

		return null; // no OwnerID & SlotIndex? f this
	}

    private void UpdateTransform(Node2D inst, EntitySnapshot entity)
    {
        if (IsInstanceValid(inst))
        {
            inst.GlobalPosition = entity.Position;
            inst.Rotation = entity.Rotation;
            inst.Scale = entity.Scale;
            inst.GetNodeOrNull<Health>("Health").health = entity.Health;
        }
    }

    private void AttachJoystick(Node2D inst, EntitySnapshot entity)
    {
        // only for local / this clients player
        bool isPlayerType = entity.Type == EntityType.DefaultPlayer 
                            || entity.Type == EntityType.Archer
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
		if (enableDebug) Debug.Print("Client: " + message);
	}
}
