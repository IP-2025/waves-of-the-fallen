using Godot;
using System;

public partial class Health : Node2D
{
	[Export] public float max_health= 110.0f;
	float CurrentHealth;
	public float CurHealth => CurrentHealth;
	public override void _Ready()
	{
		CurrentHealth = max_health;
	}
	public void Damage(float damage) 
	{
		CurrentHealth -= damage;
		if (CurrentHealth <= 0)
		{
			if(GetParent() is DefaultPlayer){
				((DefaultPlayer)GetParent()).Die();
			}
			else{
				GetParent().QueueFree();	
			}
			
		}
	}
}
