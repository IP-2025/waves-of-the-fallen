using Godot;
using System;

public partial class KunaiProjectile : Projectile
{
	public const float DefaultSpeed = 600f;
	public const int DefaultDamage = 40;
	public const int DefaultPiercing = 1;

	public float Speed { get; protected set; } = DefaultSpeed;
	public int Damage { get; protected set; } = DefaultDamage;
	public int Piercing { get; protected set; } = DefaultPiercing;

	public override void _Ready() { }
}
