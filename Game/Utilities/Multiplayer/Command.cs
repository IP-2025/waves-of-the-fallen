using Godot;
using System;

public enum CommandType { Move, Shoot, BossShop }

[Serializable]
public class Command
{
    public ulong Sequence;
    public long EntityId;
    public CommandType Type;
    public Vector2? MoveDir;
    public String Weapon;
    public int WeaponPos;

    public Command() { }
    public Command(ulong seq, long eid, CommandType type, Vector2? dir = null, String weapon = "", int weaponPos = 0)
    {
        Sequence = seq;
        EntityId = eid;
        Type = type;
        MoveDir = dir;
        Weapon = weapon;
        WeaponPos = weaponPos;
    }
}