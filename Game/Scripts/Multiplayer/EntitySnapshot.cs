using Godot;

public class EntitySnapshot
{
    // Entity
    public long NetworkId;
    public Vector2 Position;
    public float Rotation;
    public EntityType Type;

    // WaveCounter
    public int WaveCount;
    public float WaveTimeLeft;

    // Health
    public float Health;

    // Weapons
    public long? OwnerId;
    public int? SlotIndex;

    public EntitySnapshot() { }

    public EntitySnapshot(
        long id, 
        Vector2 pos, 
        float rot, 
        float health, 
        EntityType type, 
        int waveCount = 0, 
        float waveTimeLeft = 0, 
        long? ownerId = null, 
        int? slotIndex = null
    )
    {

        NetworkId = id;
        Position = pos;
        Rotation = rot;
        Type = type;
        WaveCount = waveCount;
        WaveTimeLeft = waveTimeLeft;
        Health = health;
        OwnerId = ownerId;
        SlotIndex = slotIndex;
    }
}
