using Godot;

public partial class Ranger : DefaultPlayer
{
	public Ranger()
	{
		MaxHealth = 150;
		Speed = 500.0f;  
	}

	public override void _Ready()
	{
		base._Ready();
		CurrentHealth = MaxHealth;
	}

	public override void UseAbility()
	{
		GD.Print("Platzhsalter Fähigkeit Ranger");
	}
}
