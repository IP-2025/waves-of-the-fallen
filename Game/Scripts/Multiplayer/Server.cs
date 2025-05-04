using Godot;
using System.Collections.Generic;
using System.Reflection.Metadata;


public partial class Server : Node
{
    public static Server Instance;

    private bool enableDebug = true;
    public Dictionary<long, Node2D> Entities = new Dictionary<long, Node2D>();
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
            // get move vector from command
            var dir = cmd.MoveDir.Value;
            if (dir.Length() > 1f)
            {
                dir = dir.Normalized();
            }

            // try to get the joystick node from the active player
            var joystick = entity.GetNodeOrNull<Joystick>("Joystick");
            if (joystick != null)
            {
                joystick.PosVector = dir;
                DebugIt($"Set Joystick.PosVector = {dir} on Entity {cmd.EntityId}");
            }
        }
        else if (cmd.Type == CommandType.Shoot)
        {
            // Tmaybe we need, maybe we don't
        }
    }

    private void DebugIt(string message)
    {
        if (enableDebug)
            GD.Print($"Server: {message}");
    }
}