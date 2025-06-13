using System.Diagnostics;
using Godot;

public partial class Rider : EnemyBase
{
	/// <summary>
	/// Configuration for the Rider:
	/// - stopDistance: Distance at which Rider stops moving.
	/// - attackRange: Close range distance for Rider's attack.
	/// </summary>
	[Export] public float stopDistance = 20f;
	[Export] public float attackRange = 10f;

	public Rider()
	{
		speed = 120f;
		damage = 7f;
		attacksPerSecond = 1f;
		scoreValue = 150; // z.B. 150 Punkte für einen Rider
	}

	/// <summary>
	/// Performs an attack on the player if in range.
	/// </summary>
	public override void Attack()
	{
		if (player != null)
		{
			player.GetNode<Health>("Health").Damage(damage);
			if (enableDebug)
				Debug.Print($"Rider dealt {damage} damage to the player!");
		}
	}

}
