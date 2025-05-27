using Godot;
using System;

public partial class CrossbowArrow : Projectile
{
	public const float DefaultSpeed    = 800f;
	public const int   DefaultDamage   = 100;
	public const int   DefaultPiercing = 3;

	public float Speed    { get; protected set; } = DefaultSpeed;
	public int   Piercing { get; protected set; } = DefaultPiercing;
	public int   Damage   { get; protected set; } = DefaultDamage;
}
