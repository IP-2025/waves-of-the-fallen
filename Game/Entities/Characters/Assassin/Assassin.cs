using Godot;

public partial class Assassin : DefaultPlayer
{
	public Assassin()
	{
		MaxHealth = 75;
		Speed = 500.0f;
	}

	public override void _Ready()
	{
		base._Ready();

		var healthNode = GetNode<Health>("Health");
		healthNode.max_health = MaxHealth;
		healthNode.ResetHealth();

		GD.Print($"Assassin initialized. Speed: {Speed}, MaxHealth: {MaxHealth}");
	}

	protected override void UseAbility()
	{
		//TODO: Implement Assassin's ability
	}
}