using Godot;

public partial class Assassin : DefaultPlayer
{
	public Assassin()
	{
		MaxHealth = 100; 
		Speed = 1000.0f;  
	}

	public override void UseAbility()
	{
		GD.Print("Platzhalter Fähigkeit Assassine!");
	}
}
