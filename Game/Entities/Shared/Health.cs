using Godot;
using System;
using System.Diagnostics;
using System.Linq;

public partial class Health : Node2D
{
	public bool disable = false; // in multiplayer for clients, server handles health and stuff
	[Export]
	public float max_health;
	public float health; // current health value
	public float CurHealth => health; // property to access current health

	[Signal]
	public delegate void HealthDepletedEventHandler(); // signal emitted when health is depleted

	// property to access max health
	public float MaxHealth
	{
		get => max_health;
		set => max_health = value;
	}

	private bool isDead = false;

	public override void _Ready()
	{
		health = max_health; // initialize health to max health
	}

	public void Damage(float damage)
	{
		if (isDead) return;
		if (!disable)
			health -= damage; // reduce health by damage amount
		
		doAnimation(); // client has to do damage animations
		
		if (GetParent() is EnemyBase enemy)
		{
			SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("enemyHurt"), ((Node2D)GetParent()).Position);
		}
		else if (GetParent() is DefaultPlayer player)
		{
			SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("playerHit"), ((Node2D)GetParent()).Position, -10);
		}
		

		// GD.Print($"Took damage: {damage}, current health: {health}");
	}
	
	public void Heal(float heal)
	{
		if (!disable)
			health += heal; 
		
		if (health > max_health)
			health = max_health;
		
		doAnimation();
		// TODO Sounds fehlen noch
		//SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("enemyHurt"), ((Node2D)GetParent()).Position);

	}

	private void doAnimation()
	{
		if (isDead) return;
		// hit animation
		if (GetParent() is EnemyBase enemy)
		{
			enemy.OnHit();
		}
		else if (GetParent() is DefaultPlayer player)
		{
			player.OnHit();
		}
			
		
		// death animation
		if (health <= 0) {
			GD.Print("Player died, parent: " + GetParent().Name);
			isDead = true;
			// check if parent is DefaultPlayer
			if (GetParent() is DefaultPlayer player)
			{
				player.Die(); // call Die() method if parent is DefaultPlayer
				EmitSignal(SignalName.HealthDepleted, player.OwnerPeerId);
			}
			else if (GetParent() is EnemyBase deadEnemy)
			{
				deadEnemy.OnDeath();
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
