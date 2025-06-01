using Godot;
using System;

public partial class KunaiProjectile : Projectile
{
	public const float DefaultSpeed = 600f;
	public const int DefaultDamage = 40;
	public const int DefaultPiercing = 1;
	public override void _Ready()
	{
		Speed    = DefaultSpeed;
		Damage   = DefaultDamage;
		Piercing = DefaultPiercing;
	}
}
