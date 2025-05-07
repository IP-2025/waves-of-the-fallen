using Godot;
using System;

public partial class KunaiProjectile : Projectile
{
	public override void _Ready()
	{
		Speed = 600;
		Piercing = 1;
		Damage = 40;
	}
}
