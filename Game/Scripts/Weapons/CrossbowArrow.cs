using Godot;
using System;

public partial class CrossbowArrow : Projectile
{
	public override void _Ready()
	{
		Speed = 800;
		Piercing = 3;
		Damage = 100;
	}
	
}
