using Godot;
using System.Collections.Generic;

public class AbilitySnapshot
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

	// Scores
	public Dictionary<long, int> PlayerScores;

	public AbilitySnapshot() {}

	public AbilitySnapshot(
		long id,
		float health,
		EntityType type,
		long? ownerId = null
	)
	{
		NetworkId = id;
		Health = health;
		Type = type;
		OwnerId = ownerId;
	}	//Vector2 pos,	//Position = pos;
}
