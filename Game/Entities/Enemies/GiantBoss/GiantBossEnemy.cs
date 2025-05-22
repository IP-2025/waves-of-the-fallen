using Godot;
using System;
using System.Diagnostics;

public partial class GiantBossEnemy : EnemyBase
{
	public GiantBossEnemy()
	{
		speed = 40f;
		damage = 10f;
		attacksPerSecond = 3f;
	}
	public override void Attack()
	{
		player.GetNode<Health>("Health").Damage(damage);

		if (enableDebug)
		{
			Debug.Print($"Giant Boss attacks (melee) with speed: {speed}, damage: {damage}, attacksPerSecond: {attacksPerSecond}!");
		}
	}
}
