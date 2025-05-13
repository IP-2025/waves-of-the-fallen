using Godot;

public partial class Mage : DefaultPlayer
{
	public Mage()
	{
		MaxHealth = 120;
		Speed = 200.0f;
	}

	public override void _Ready()
	{
		base._Ready();

		var healthNode = GetNode<Health>("Health");
		healthNode.max_health = MaxHealth;
		healthNode.ResetHealth();

		GD.Print($"Mage initialized. Speed: {Speed}, MaxHealth: {MaxHealth}");
	}

	public override void UseAbility()
	{
		//TODO: Implement Mage's ability
	}
}
