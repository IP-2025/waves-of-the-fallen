using Godot;

public partial class Knight : DefaultPlayer
{
	public Knight()
	{
		MaxHealth = 200; 
		Speed = 400.0f;  
	}
	
	public override void _Ready()
	{
		base._Ready();

		// Synchronize MaxHealth with the Health node
		var healthNode = GetNodeOrNull<Health>("Health");
		if (healthNode != null)
		{
			healthNode.max_health = MaxHealth; // Set the max_health value in the Health node
			healthNode.ResetHealth(); // Reset current health to max_health
		}
		else
		{
			GD.PrintErr("Health node not found on Knight!");
		}

		GD.Print($"Knight initialized. Speed: {Speed}, MaxHealth: {MaxHealth}");
	}

	public override void UseAbility()
	{
		GD.Print("Placeholder ability for Knight");
	}
}
