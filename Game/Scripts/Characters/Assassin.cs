using Godot;

public partial class Assassin : DefaultPlayer
{
	public Assassin()
	{
		MaxHealth = 75;
		Speed = 1000.0f;
	}

	public override void _Ready()
	{
		base._Ready();

		// Synchronize MaxHealth with the Health node
		var healthNode = GetNode<Health>("Health");
		healthNode.max_health = MaxHealth;
		healthNode.ResetHealth();

		GD.Print($"Assassin initialized. Speed: {Speed}, MaxHealth: {MaxHealth}");
	}

	public override void UseAbility()
	{
		GD.Print("Platzhalter Fähigkeit Assassine");
	}
}
