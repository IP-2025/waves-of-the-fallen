using Godot;
using System;

public partial class Health : Node2D
{
	[Signal]
	public delegate void HealthDepletedEventHandler();
	
	[Export] float max_health = 100f;
	float health;

	// Öffentliche Eigenschaft, um max_health lesbar zu machen
	public float MaxHealth => max_health;

	public override void _Ready()
	{
		health = max_health;
	}

	public void Damage(float damage) 
	{
		health -= damage;

		if (health <= 0)
		{
			EmitSignal(SignalName.HealthDepleted);
			GetParent().QueueFree();
		}
	}
}
