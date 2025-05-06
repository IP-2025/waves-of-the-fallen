using Godot;
using System;

public partial class Health : Node2D
{
	[Export] public float max_health = 100.0f; // maximum health value
	private float health; // current health value
	public float CurHealth => health; // property to access current health

	[Signal]
	public delegate void HealthDepletedEventHandler(); // signal emitted when health is depleted

	// property to access max health
	public float MaxHealth => max_health;

	public override void _Ready()
	{
		health = max_health; // initialize health to max health
	}

	public void Damage(float damage) 
	{
		health -= damage; // reduce health by damage amount
		if (health <= 0)
		{
			// check if parent is DefaultPlayer
			if (GetParent() is DefaultPlayer player)
			{
				player.Die(); // call Die() method if parent is DefaultPlayer
			}
			else
			{
				GetParent().QueueFree(); // otherwise, free the parent node
			}

			EmitSignal(SignalName.HealthDepleted); // emit signal when health is depleted
		}
	}

	public void ResetHealth()
	{
		health = max_health;
	}
}
