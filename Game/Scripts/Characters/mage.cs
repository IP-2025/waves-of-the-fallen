using Godot;

public partial class Mage : DefaultPlayer
{
	public Mage()
	{
		MaxHealth = 120;
		Speed = 200.0f;
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
