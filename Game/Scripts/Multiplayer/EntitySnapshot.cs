using Godot;

public class EntitySnapshot
{
    public long NetworkId;
    public Vector2 Position;
    public float Rotation;
    public EntityType Type;

    public EntitySnapshot(){}
    
    public EntitySnapshot(long id, Vector2 pos, float rot, EntityType type)
    {
        NetworkId = id;
        Position = pos;
        Rotation = rot;
        Type = type;
    }
}
