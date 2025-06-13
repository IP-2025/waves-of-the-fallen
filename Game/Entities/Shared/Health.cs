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

	public float MaxHealth => max_health; // property to access max health

	private bool isDead = false;

	public override void _Ready()
	{
		health = max_health; // initialize health to max health
	}

	public void Damage(float damage)
	{
		if (isDead) return;
		if (!disable)
			health -= damage;

		doAnimation();

		SoundManager.Instance.PlaySoundAtPosition(
			SoundManager.Instance.GetNode<AudioStreamPlayer2D>("enemyHurt"),
			((Node2D)GetParent()).Position
		);
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
		if (health <= 0)
		{
			isDead = true;

			EmitSignal(SignalName.HealthDepleted);

			if (GetParent() is DefaultPlayer player)
			{
				player.CallDeferred("Die");
			}
			else if (GetParent() is EnemyBase deadEnemy)
			{
				deadEnemy.OnDeath();
			}
			else
			{
				Node parent = GetParent();
				while (parent != null && parent is not DefaultPlayer)
				{
					parent = parent.GetParent();
				}

				if (parent is DefaultPlayer defaultPlayer)
				{
					GD.Print("DefaultPlayer found higher in tree, calling Die()");
					defaultPlayer.Die();
				}
				else
				{
					GD.Print("Unknown parent type, calling QueueFree()");
					GetParent().QueueFree();
				}
			}
		}
	}

	public void ResetHealth()
	{
		health = max_health;
	}
}
