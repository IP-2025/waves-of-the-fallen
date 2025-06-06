using Godot;
using System;
using System.Diagnostics;

public partial class BasicEnemy : EnemyBase
{
	public BasicEnemy()
	{
		speed = 150f;
		damage = 5f;
		attacksPerSecond = 1f;
		scoreValue = 50; // z.B. 50 Punkte für einen Goblin
	}
	public override void Attack() 
	{
		player.GetNode<Health>("Health").Damage(damage);

		if (enableDebug)
		{
			Debug.Print($"BasicEnemy attacks (melee) with speed: {speed}, damage: {damage}, attacksPerSecond: {attacksPerSecond}!");
		}
	}
}
