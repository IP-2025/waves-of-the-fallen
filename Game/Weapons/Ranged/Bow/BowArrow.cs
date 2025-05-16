using Godot;
using System;

public partial class BowArrow : Projectile
{
	public override void _Ready()
	{
		Speed = 1200;
		Piercing = 1;
		Damage = 50;
	}
}
