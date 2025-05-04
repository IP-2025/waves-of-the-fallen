using Godot;

public partial class EnemyPattern : Node2D
{
	// EnemyPattern serves as a class to be used in the editor to assign values to quickly assign values to all enemies within the pattern
	// EnemyPattern can be expanded if required

	[Export(PropertyHint.Range, "1,15,0.5,or_greater,or_less")] // sets the spawning cost, which has to be rolled in order for the pattern to spawn (currently dependant on waveCount)
	public float spawningCost = 1;

	[Export(PropertyHint.Range, "0.1,10,0.1,or_greater,or_less")] // sets the enemy health multiplier
	public float healthMultiplier = 1;

	[Export(PropertyHint.Range, "0.1,10,0.1,or_greater,or_less")] // sets the enemy speed multiplier
	public float speedMultiplier = 1;

	[Export(PropertyHint.Range, "1,15,1,or_greater,or_less")] // used to set the minimum wave at which the pattern can spawn
	public int minWave = 1;

	[Export(PropertyHint.Range, "1,15,1,or_greater,or_less")] // used to set the minimum wave at which the pattern can spawn
	public int maxWave = 15;
}
