using Godot;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Client : Node
{
    bool enableDebug = true;
    // GameRoot container for entities
    private Node2D _entitiesRoot;
    private Dictionary<long, Node2D> _instances = new();

    // mapping per entity type
    private Dictionary<EntityType, PackedScene> _prefabs = new()
    {
        { EntityType.Player, GD.Load<PackedScene>("res://Scenes/Characters/default_player.tscn") },
        { EntityType.Enemy,  GD.Load<PackedScene>("res://Scenes/Enemies/Enemy.tscn") },
    };

    public override void _Ready()
    {
        // create container under GameRoot
        _entitiesRoot = new Node2D { Name = "Entities" };
        GetParent().AddChild(_entitiesRoot);
    }

    public void ApplySnapshot(Snapshot snap)
    {
        var seen = new HashSet<long>();

        // every entity in snapshot
        foreach (var es in snap.Entities)
        {
            seen.Add(es.NetworkId);

            // instanciate entity if not already done
            if (!_instances.TryGetValue(es.NetworkId, out var inst))
            {
                if (!_prefabs.TryGetValue(es.Type, out var scene))
                {
                    GD.PrintErr($"Cant find path for {es.Type}");
                    continue;
                }

                inst = scene.Instantiate<Node2D>();
                inst.Name = $"E_{es.NetworkId}";
                _entitiesRoot.AddChild(inst);
                _instances[es.NetworkId] = inst;

                DebugIt($"Instantiated entity of type {es.Type} with NetworkId {es.NetworkId}");
            }

            // update and transform
            inst.GlobalPosition = es.Position;
            inst.Rotation = es.Rotation;
        }

        // cleanup: remove entities not in snapshot
        foreach (var id in new List<long>(_instances.Keys))
        {
            if (!seen.Contains(id))
            {
                _instances[id].QueueFree();
                _instances.Remove(id);
            }
            
            DebugIt($"Deleted entity with NetworkId {id}");
        }
    }

    private void DebugIt(string message)
    {
        if (enableDebug) Debug.Print("Client: " + message);
    }
}
