using Godot;

public partial class Healer : DefaultPlayer
{
	public Healer()
	{
		MaxHealth = 200;
		Speed = 150f;  
	}

	public override void _Ready()
	{
		base._Ready();
		CurrentHealth = MaxHealth;
	}
	public override void UseAbility()
	{
		GD.Print("Platzhalter Fähigkeit Healer");
	}
}
