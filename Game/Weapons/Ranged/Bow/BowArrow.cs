using Godot;
using System;

public partial class BowArrow : Projectile
{
	public const float DefaultSpeed  = 1200f;
	public const int   DefaultDamage = 50;
	public const int   DefaultPiercing = 1;

	public float Speed    { get; protected set; } = DefaultSpeed;
	public int   Piercing { get; protected set; } = DefaultPiercing;
	public int   Damage   { get; protected set; } = DefaultDamage;
}
