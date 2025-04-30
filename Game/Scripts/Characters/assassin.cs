using Godot;

public partial class Assassin : DefaultPlayer
{
	public Assassin()
	{
		MaxHealth = 110; 
		Speed = 200.0f;  
	}
	
	public override void _Ready()
	{
		base._Ready();
		CurrentHealth = MaxHealth;
	}
	public override void UseAbility()
	{
		GD.Print("Platzhalter Fähigkeit Assassine!");
	}
}
