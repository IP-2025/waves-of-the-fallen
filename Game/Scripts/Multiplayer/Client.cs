using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class Client : Node
{
    // NetworkId → Node2D (Player-Instanzen)
    private Dictionary<long, Node2D> _entities = new();

    // Vom Server aufgerufen, um den Host-Player anzulegen
    public void SpawnLocalPlayer()
    {
        long myId = GetTree().GetMultiplayer().GetUniqueId();
        CreateEntity(myId, Vector2.Zero, 0f, isLocal: true);
    }

    // Wird vom NetworkManager bei jedem Snapshot aufgerufen
    public void ApplySnapshot(Snapshot snap)
    {
        long myId = GetTree().GetMultiplayer().GetUniqueId();
        var activeIds = snap.Entities.Select(es => es.NetworkId).ToHashSet();

        // Update oder Erstellen
        foreach (var es in snap.Entities)
        {
            bool isLocal = es.NetworkId == myId;
            if (_entities.TryGetValue(es.NetworkId, out var node))
            {
                // schon vorhanden: nur Position/Rotation updaten
                node.Position = es.Position;
                node.Rotation = es.Rotation;
            }
            else
            {
                // neu anlegen
                //CreateEntity(es.NetworkId, es.Position, es.Rotation, isLocal);
            }
        }

        // Entfernen nicht mehr existierender IDs
        foreach (var stale in _entities.Keys.Except(activeIds).ToList())
        {
            _entities[stale].QueueFree();
            _entities.Remove(stale);
        }
    }

    private void CreateEntity(long id, Vector2 pos, float rot, bool isLocal)
    {
        var player = GD.Load<PackedScene>("res://Scenes/Characters/default_player.tscn").Instantiate<Node2D>();
        player.Name = id.ToString();
        player.Name = $"Player_{id}";

        // add joystick to player if this player is local player
        if (isLocal)
        {
            var joystick = GD.Load<PackedScene>("res://Scenes/Joystick/joystick.tscn").Instantiate<Node2D>();
            player.AddChild(joystick);
        }

        player.Position = pos;
        player.Rotation = rot;
        GetTree().Root.AddChild(player);
        _entities[id] = player;
    }
}
