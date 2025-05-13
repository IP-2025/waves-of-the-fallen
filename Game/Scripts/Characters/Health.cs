using Godot;
using System;
using System.Diagnostics;
using System.Linq;

public partial class Health : Node2D
{
	public bool disable = false; // in multiplayer for clients, server handles health and stuff
	public float max_health;
	public float health; // current health value
	public float CurHealth => health; // property to access current health

	[Signal]
	public delegate void HealthDepletedEventHandler(); // signal emitted when health is depleted

	// property to access max health
	public float MaxHealth => max_health;
	private bool isDead = false;

	public override void _Ready()
	{
		health = max_health; // initialize health to max health
	}

	public void Damage(float damage)
	{
		doAnimation(); // client has to do damage animations
		
		SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("enemyHurt"), ((Node2D)GetParent()).Position);

		if (disable) return; // Client cant do damage to enemies... server handles the damage

		health -= damage; // reduce health by damage amount
		GD.Print($"Took damage: {damage}, current health: {health}");
	}

	private void doAnimation()
	{
		// hit animation
		if (GetParent() is EnemyBase enemy)
			enemy.OnHit();

		// death animation
		if (health <= 0) {
			// check if parent is DefaultPlayer
			if (GetParent() is DefaultPlayer player)
				player.Die(); // call Die() method if parent is DefaultPlayer
			else if (GetParent() is EnemyBase deadEnemy)
				deadEnemy.OnDeath();
			else
				GetParent().QueueFree(); // otherwise, free the parent node

			EmitSignal(SignalName.HealthDepleted); // emit signal when health is depleted
			}
	}

	public void ResetHealth()
	{
		health = max_health;
	}
}
