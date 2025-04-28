using Godot;

public partial class Mage : DefaultPlayer
{
	public Mage()
	{
<<<<<<< HEAD
		MaxHealth = 120;
		Speed = 200.0f;
=======
		MaxHealth = 50;
		Speed = 200.0f;  //Sehr niedrig für Testzwecke
>>>>>>> 3ba17a384a46bb450c850f3020d66250fdb953a0
	}

	public override void _Ready()
	{
		base._Ready();
		CurrentHealth = MaxHealth;
	}
	public override void UseAbility()
	{
		GD.Print("Platzhalter Fähigkeit Magier");
	}
	public override void Die()
	{
		GD.Print("Mage death animation playing...");
		//animationPlayer.Play("mage_death");
	}
}
