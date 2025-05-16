using Godot;
using System;

public enum CommandType { Move, Shoot }

[Serializable]
public class Command
{
    public ulong Sequence;
    public long EntityId;
    public CommandType Type;
    public Vector2? MoveDir;

    public Command() { }
    public Command(ulong seq, long eid, CommandType type, Vector2? dir = null)
    {
        Sequence = seq;
        EntityId = eid;
        Type = type;
        MoveDir = dir;
    }
}