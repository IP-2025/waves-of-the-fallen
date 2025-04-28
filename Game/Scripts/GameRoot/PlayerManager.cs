using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PlayerManager : Node
{
	private Dictionary<long, DefaultPlayer> _players = new();

    public void ApplySnapshot(Snapshot snap)
    {
        // 1) Spawn neue Spieler
        foreach (var es in snap.Entities.Where(e => e.Type == EntityType.Player))
        {
            if (!_players.ContainsKey(es.NetworkId))
                SpawnRemotePlayer(es.NetworkId);
        }

        // 2) Update Position/Rotation
        foreach (var es in snap.Entities.Where(e => e.Type == EntityType.Player))
        {
            var p = _players[es.NetworkId];
            p.GlobalPosition = es.Position;
            p.Rotation = es.Rotation;
        }

        // 3) Entferne herausgefallene
        var toRemove = _players.Keys.Except(
            snap.Entities.Where(e=> e.Type==EntityType.Player).Select(e=>e.NetworkId)
        ).ToList();
        foreach (var id in toRemove)
        {
            _players[id].QueueFree();
            _players.Remove(id);
        }
    }

    private void SpawnRemotePlayer(long netId)
    {
        var scene = GD.Load<PackedScene>("res://Scenes/Characters/default_player.tscn");
        var p = scene.Instantiate<DefaultPlayer>();
        p.Name = $"Player_{netId}";
        AddChild(p);
        _players[netId] = p;
    }
}