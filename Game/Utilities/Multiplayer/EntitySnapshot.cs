using Godot;

public class EntitySnapshot
{
	// Entity
	public long NetworkId;
	public Vector2 Position;
	public float Rotation;
	public Vector2 Scale;
	public EntityType Type;

	// WaveCounter
	public int WaveCount;
	public float WaveTimeLeft;
	public bool GraceTime;

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
		Vector2 scale,
		float health, 
		EntityType type, 
		int waveCount = 0, 
		float waveTimeLeft = 0,
		bool graceTime = false,
		long? ownerId = null, 
		int? slotIndex = null
	)
	{

		NetworkId = id;
		Position = pos;
		Rotation = rot;
		Scale = scale;
		Type = type;
		WaveCount = waveCount;
		WaveTimeLeft = waveTimeLeft;
		GraceTime = graceTime;
		Health = health;
		OwnerId = ownerId;
		SlotIndex = slotIndex;
	}
}
