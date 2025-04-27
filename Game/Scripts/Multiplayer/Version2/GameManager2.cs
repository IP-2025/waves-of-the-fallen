using Godot;
using System.Collections.Generic;
using System.Reflection.Metadata;


public partial class GameManager2 : Node {
    public static GameManager2 Instance;
    public Dictionary<long, Node2D> Entities = new Dictionary<long, Node2D>();
    private long _nextId = 1;

    public override void _Ready() {
        Instance = this;
        // Karte laden
        GetNode<MapLoader>("MapLoader").LoadMap("res://Maps/level1.tscn");
        // Player-Spawn
        SpawnPlayer(GetTree().GetMultiplayer().GetUniqueId());
        // Enemy-Spawner starten
        GetNode<EnemySpawner>("EnemySpawner").Start();
    }

    public void ProcessCommand(Command cmd) {
        if (!Entities.ContainsKey(cmd.EntityId)) return;
        var entity = Entities[cmd.EntityId];
        switch (cmd.Type) {
            case CommandType.Move:
                entity.Position += cmd.MoveDir.Value * 10f * NetworkManager.GetTickDelta();
                break;
            case CommandType.Shoot:
                // handle shoot
                break;
        }
    }

    public long SpawnPlayer(long peerId) {
        var scene = GD.Load<PackedScene>("res://Actors/Player.tscn");
        var p = scene.Instantiate<Node2D>();
        p.Name = "Player_" + peerId;
        AddChild(p);
        Entities[_nextId] = p;
        return _nextId++;
    }

    public long GetNextId() {
        return _nextId++;
    }
}