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

		var healthNode = GetNodeOrNull<Health>("Health");
		if (healthNode != null)
		{
			GD.Print($"Before sync: Health Node MaxHealth: {healthNode.max_health}, Knight MaxHealth: {MaxHealth}");
			healthNode.max_health = MaxHealth;
			healthNode.ResetHealth();
			GD.Print($"After sync: Health Node MaxHealth: {healthNode.max_health}, CurrentHealth: {healthNode.CurHealth}");
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
