using Godot;
using System;
using System.Diagnostics;

public partial class Health : Node2D
{
	[Signal]
	public delegate void HealthDepletedEventHandler();
	//[Signal]
	//public delegate void EnemyDamageEventHandler();
	
	[Export] float max_health = 100f;
	float health;

	public override void _Ready()
	{
		health = max_health;
	}

	public void Damage(float damage) 
	{
		health -= damage;

		if(!GetParent().Name.ToString().Contains("Player")) 
		{
			//Debug.Print(GetParent().Name);
			SoundManager.Instance.PlaySFX(GD.Load<AudioStream>("res://Assets/Sounds/test.wav"));
			//EmitSignal(SignalName.EnemyDamage);
			/*var player = GetNode<AudioStreamPlayer2D>("EnemyHurt");
			Debug.Print("hit");
			if(player != null) 
			{
				Debug.Print("sound should be playing");
				player.Stream = GD.Load<AudioStream>("res://Assets/Sounds/test.wav");
				player.Play();
			}*/
		}

		if (health <= 0)
		{
			//GetTree().Paused = true;
			EmitSignal(SignalName.HealthDepleted);
			GetParent().QueueFree();
			
		}
	}
}
