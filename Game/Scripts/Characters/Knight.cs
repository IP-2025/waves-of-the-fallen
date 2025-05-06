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
		CurrentHealth = MaxHealth;
		GD.Print($"Knight _Ready called. Speed: {Speed}, MaxHealth: {MaxHealth}");
	}

	public override void UseAbility()
	{
		GD.Print("Placeholder ability for Knight");
	}
}
