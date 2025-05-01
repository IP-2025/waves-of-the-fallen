using Godot;

public partial class EnemyPattern : Node2D{

	[Export]
	public float spawningCost { get; set; } = 1;

	[Export]
	public float healthScaling = 1;
	
	[Export]
	public float speedScaling = 1;

}
