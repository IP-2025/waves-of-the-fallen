using Godot;
using System;

public partial class Mount : CharacterBody2D
{
	[Export] public int Health = 100;

	[Signal] public delegate void DefeatedEventHandler();

	public void TakeDamage(int amount)
	{
		Health -= amount;
		if (Health <= 0)
		{
			EmitSignal(nameof(Defeated));
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		// Add movement logic for the mount here
	}
}
