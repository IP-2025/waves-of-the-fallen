using Godot;
using System;

public partial class Health : Node2D
{
	[Export] float max_health = 10f;
	float health;

	public override void _Ready()
	{
		health = max_health;
	}

	public void Damage(float damage) 
	{
		health -= damage;

		if (health <= 0)
		{
			GetParent().QueueFree();
		}
	}
}
