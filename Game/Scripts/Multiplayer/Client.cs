using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
        { EntityType.DefaultPlayer, GD.Load<PackedScene>("res://Scenes/Characters/default_player.tscn") },
        { EntityType.Archer, GD.Load<PackedScene>("res://Scenes/Characters/archer.tscn") },
        { EntityType.DefaultEnemy,  GD.Load<PackedScene>("res://Scenes/Characters/default_enemy.tscn") },
        { EntityType.RangedEnemy,  GD.Load<PackedScene>("res://Scenes/Characters/ranged_enemy.tscn") },
    };

    public override void _Ready()
    {
    }

    public Command GetCommand(int tick)
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
        foreach (var entity in entities)
        {
            // HUD / WaveCounter stuff
            if (!_waveTimerReady && _camera != null)
            {
                var wt = GD.Load<PackedScene>("res://Scenes/Waves/WaveTimer.tscn").Instantiate<WaveTimer>();
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
    }

    private Node2D CreateInstance(EntitySnapshot entity)
    {
        if (!_prefabs.TryGetValue(entity.Type, out var scene))
        {
            GD.PrintErr($"Cant find path for {entity.Type}");
            return null;
        }

        var inst = scene.Instantiate<Node2D>();

        // sisable health for enemies because server handles it
        if (entity.Type == EntityType.DefaultEnemy || entity.Type == EntityType.RangedEnemy || entity.Type == EntityType.DefaultPlayer || entity.Type == EntityType.Archer)
        {
            var helthNode = inst.GetNodeOrNull<Health>("Health");
            helthNode.disable = true;
            helthNode.health = entity.Health * 100; // high value so that client cant kill and cant be killed. Server handles it
        }

        inst.Name = $"E_{entity.NetworkId}";

        GetNode<GameRoot>("/root/GameRoot").AddChild(inst);
        return inst;
    }

    private void UpdateTransform(Node2D inst, EntitySnapshot entity)
    {
        if (GodotObject.IsInstanceValid(inst))
        {
            inst.GlobalPosition = entity.Position;
            inst.Rotation = entity.Rotation;
            inst.GetNodeOrNull<Health>("Health").health = entity.Health;
        }
    }

    private void AttachJoystick(Node2D inst, EntitySnapshot entity)
    {
        // only for local / this clients player
        bool isPlayerType = entity.Type == EntityType.DefaultPlayer || entity.Type == EntityType.Archer;
        if (!isPlayerType || entity.NetworkId != Multiplayer.GetUniqueId())
        {
            return;
        }

        var joystick = GD.Load<PackedScene>("res://Scenes/Joystick/joystick.tscn").Instantiate<Node2D>();
        inst.AddChild(joystick);
        DebugIt($"Joystick added to player with ID {entity.NetworkId}");
    }

    private void ChangeCamera(Node2D inst, EntitySnapshot entity)
    {
        // only for local / this clients player
        if (entity.Type != EntityType.DefaultPlayer && entity.Type != EntityType.Archer)
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
