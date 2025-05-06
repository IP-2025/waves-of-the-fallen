using Godot;

public partial class Mage : DefaultPlayer
{
	public Mage()
	{
		MaxHealth = 130;
		Speed = 250.0f;
	}

	public override void _Ready()
	{
		base._Ready();

		// Synchronize MaxHealth with the Health node
		var healthNode = GetNodeOrNull<Health>("Health");
		if (healthNode != null)
		{
			healthNode.max_health = MaxHealth; // Set the max_health value in the Health node
			healthNode.ResetHealth(); // Reset current health to max_health
		}
		else
		{
			GD.PrintErr("Health node not found on Mage!");
		}

		GD.Print($"Mage initialized. Speed: {Speed}, MaxHealth: {MaxHealth}");
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
