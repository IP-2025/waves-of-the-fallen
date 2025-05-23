using Godot;

public partial class Knight : DefaultPlayer
{
	public Knight()
	{
		MaxHealth = 250; 
		Speed = 175.0f;  
	}
	
	public override void _Ready()
	{
		base._Ready();

		var healthNode = GetNode<Health>("Health");
		healthNode.max_health = MaxHealth;
		healthNode.ResetHealth();

		GD.Print($"Knight initialized. Speed: {Speed}, MaxHealth: {MaxHealth}");
	}

	protected override void UseAbility()
	{
		//TODO: Implement Knight's ability
	}
}
