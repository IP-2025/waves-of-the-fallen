using Godot;
using System;

public partial class Health : Node2D
{
	[Export] float max_health = 100f;
	float cur_health;
	public float CurHealth => cur_health;

	public override void _Ready()
	{
		cur_health = max_health;
	}
	public void Damage(float damage) 
	{
		cur_health -= damage;
		GD.Print("Player is taking damage");
		if (cur_health <= 0)
		{
			GD.Print("Player died");
			GetParent().QueueFree();
		}
	}
}
