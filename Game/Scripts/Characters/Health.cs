using Godot;
using System;

public partial class Health : Node2D
{
	public bool disable = false;
	[Export] public float max_health= 100.0f;
	public float health;
	public float CurHealth => health;
	[Signal]
	public delegate void HealthDepletedEventHandler();
	

	public override void _Ready()
	{
		health = max_health;
	}
	public void Damage(float damage) 
	{
		if (disable) return; // for client side.. server handles the damage

		health -= damage;
		if (health <= 0)
		{
			if(GetParent() is DefaultPlayer){
				((DefaultPlayer)GetParent()).Die();
			}
			else{
				GetParent().QueueFree();	
			}
			//GetTree().Paused = true;
			EmitSignal(SignalName.HealthDepleted);
			GetParent().QueueFree();
			
		}
	}
}
