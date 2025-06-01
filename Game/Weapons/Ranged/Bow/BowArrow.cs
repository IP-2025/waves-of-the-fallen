using Godot;
using System;

public partial class BowArrow : Projectile
{
	public const float DefaultSpeed = 1200f;
	public const int DefaultDamage = 50;
	public const int DefaultPiercing = 1;

	public override void _Ready()
	{
		Speed    = DefaultSpeed;
		Damage   = DefaultDamage;
		Piercing = DefaultPiercing;
	}
}
