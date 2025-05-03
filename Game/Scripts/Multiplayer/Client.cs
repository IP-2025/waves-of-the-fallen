using Godot;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Client : Node
{
    bool enableDebug = true;
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

    public void ApplySnapshot(Snapshot snap)
    {
        var networkIDs = new HashSet<long>();

        // instanciate or update entities
        foreach (var entity in snap.Entities)
        {
            networkIDs.Add(entity.NetworkId);

            if (!_instances.TryGetValue(entity.NetworkId, out var inst))
            {
                // create new instance
                if (!_prefabs.TryGetValue(entity.Type, out var scene))
                {
                    GD.PrintErr($"Cant find path for {entity.Type}");
                    continue;
                }
                inst = scene.Instantiate<Node2D>();
                inst.Name = $"E_{entity.NetworkId}";
                GetNode<GameRoot>("/root/GameRoot").AddChild(inst);
                _instances[entity.NetworkId] = inst;

                DebugIt($"Instanciate {entity.Type} with ID {entity.NetworkId}");
            }

            // set position and rotation
            if (IsInstanceValid(inst))
            {
                inst.GlobalPosition = entity.Position;
                inst.Rotation = entity.Rotation;
            }
        }

        // clean up removed entities
        var toRemove = new List<long>();
        foreach (var id in _instances.Keys)
        {
            if (!networkIDs.Contains(id))
            {
                var node = _instances[id];

                if (node == null || !IsInstanceValid(node))
                {
                    toRemove.Add(id);
                }
            }
        }

        foreach (var id in toRemove)
        {
            var nodeToRemove = _instances[id];
            if (nodeToRemove != null && IsInstanceValid(nodeToRemove))
            {
                nodeToRemove.QueueFree();
                _instances.Remove(id);
                DebugIt($"Removed entity with ID {id}");
            }
        }
    }


    private void DebugIt(string message)
    {
        if (enableDebug) Debug.Print("Client: " + message);
    }
}
