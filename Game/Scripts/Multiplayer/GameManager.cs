using Godot;
using System.Collections.Generic;
using System.Reflection.Metadata;


public partial class GameManager : Node {
    public static GameManager Instance;
    public Dictionary<long, Node2D> Entities = new Dictionary<long, Node2D>();
    private long _nextId = 1;
    public static List<PlayerInfo> Players = new List<PlayerInfo>();

    public override void _Ready() {
        Instance = this;
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

    public long GetNextId() {
        return _nextId++;
    }
}