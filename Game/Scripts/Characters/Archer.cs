using Godot;

public partial class Archer : DefaultPlayer
{
	public Archer()
	{
		MaxHealth = 175;
		Speed = 500.0f;  
	}

	public override void _Ready()
	{
		base._Ready();

		// Synchronize MaxHealth with the Health node
		var healthNode = GetNode<Health>("Health");
		healthNode.max_health = MaxHealth; // Set the max_health value in the Health node
		healthNode.ResetHealth(); // Reset current health to max_health

		GD.Print($"Archer initialized. Speed: {Speed}, MaxHealth: {MaxHealth}");
	}

	public override void UseAbility()
	{
		GD.Print("Platzhalter Fähigkeit Ranger");
	}
}
