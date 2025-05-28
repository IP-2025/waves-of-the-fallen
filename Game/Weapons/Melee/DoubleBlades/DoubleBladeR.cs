using Godot;
using System;
public partial class DoubleBladeR : MeleeWeapon
{
	private AnimationPlayer DoubleBladeRAnimationPlayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DoubleBladeRAnimationPlayer = GetNode<AnimationPlayer>("DoubleBladeR/AnimationPlayerR");
		MeleeDamage = 100;
		WeaponRange = 100;
	}

	public void StartAttack()
	{ 
		ShootMeleeVisual(() =>
	{
		DoubleBladeRAnimationPlayer.Play("BladeRAttack");
	});
	}
	
}
