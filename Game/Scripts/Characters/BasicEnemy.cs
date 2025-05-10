using Godot;
using System;
using System.Diagnostics;

public partial class BasicEnemy : EnemyBase
{
	public override void Attack() 
	{
		player.GetNode<Health>("Health").Damage(damage);
		Debug.Print("BasicEnemy attacks (melee)!");
	}
}
