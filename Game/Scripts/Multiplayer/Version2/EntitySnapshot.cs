using Godot;
using System;

[Serializable]
public class EntitySnapshot {
    public long NetworkId;
    public Vector2 Position;
    public float Rotation;
    public EntitySnapshot(long id, Vector2 pos, float rot) {
        NetworkId = id; Position = pos; Rotation = rot;
    }
}