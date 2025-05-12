using Godot;

public partial class Warrior : DefaultPlayer
{
	public Warrior()
	{
		MaxHealth = 200; 
		Speed = 400.0f;  
	}
	
	public override void _Ready()
	{
		base._Ready();
		CurrentHealth = MaxHealth;
	}

	public override void UseAbility()
	{
		GD.Print("Platzhalter Fähigkeit Krieger");
	}
}
