using Godot;
using System;

public partial class CrossbowArrow : Projectile
{
	public const float DefaultSpeed    = 800f;
	public const int   DefaultDamage   = 100;
	public const int   DefaultPiercing = 3;

	public override void _Ready()
	{
		Speed    = DefaultSpeed;
		Damage   = DefaultDamage;
		Piercing = DefaultPiercing;
	}
}
