using Godot;
using System;
public partial class DoubleBladeR : MeleeWeapon
{
	private AnimationPlayer DoubleBladeRAnimationPlayer;
	private Sprite2D SwordTrailTest;

	public override void _Ready()
	{
		DoubleBladeRAnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayerR");
		SwordTrailTest = GetNode<Sprite2D>("SwordTrailTest");
		MeleeDamage = 100;
		WeaponRange = 150;
		SwordTrailTest.Visible = false;
	}

	public void StartAttack()
	{ 
		ShootMeleeVisual(() =>
	{
		DoubleBladeRAnimationPlayer.Play("BladeRAttack");
	});
	}
	
}
