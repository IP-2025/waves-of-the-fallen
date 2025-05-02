using Godot;
using System.Collections.Generic;
using System.Reflection.Metadata;


public partial class Server : Node
{
    public static Server Instance;
    public Dictionary<long, Node2D> Entities = new Dictionary<long, Node2D>();
    private long _nextId = 1;
    public override void _Ready()
    {
        Instance = this;
    }

    public void InitServer()
    {
        // change scene to game
        var gameScene = GD.Load<PackedScene>("res://Scenes/GameRoot/GameRoot.tscn");
        gameScene.Instantiate<Node>();
        GetTree().ChangeSceneToPacked(gameScene);
    }


    public void ProcessCommand(Command cmd)
    {
        if (!Entities.ContainsKey(cmd.EntityId)) return;
        var entity = Entities[cmd.EntityId];
        switch (cmd.Type)
        {
            case CommandType.Move:
                entity.Position += cmd.MoveDir.Value * 10f * NetworkManager.GetTickDelta();
                break;
            case CommandType.Shoot:
                // handle shoot and so on
                break;
        }
    }

    public long GetNextId()
    {
        return _nextId++;
    }
}