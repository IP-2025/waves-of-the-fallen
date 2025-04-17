using Godot;

public partial class Mage : DefaultPlayer
{
	public Mage()
	{
		MaxHealth = 120;
		Speed = 20.0f;  //Sehr niedrig für Testzwecke
	}

	public override void UseAbility()
	{
		GD.Print("Platzhalter Fähigkeit Magier");
	}
}
