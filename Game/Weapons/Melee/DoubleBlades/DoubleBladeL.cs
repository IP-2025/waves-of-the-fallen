using Godot;
using System;
public partial class DoubleBladeL : MeleeWeapon
{
	private AnimationPlayer DoubleBladeLAnimationPlayer;
	private Sprite2D SwordTrailTest;

	public override void _Ready()
	{
		DoubleBladeLAnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayerL");
		SwordTrailTest = GetNode<Sprite2D>("SwordTrailTest");
		MeleeDamage = 50;
		WeaponRange = 100;
		SwordTrailTest.Visible = false;
	}

	public void StartAttack()
	{ 
		ShootMeleeVisual(() =>
	{
		DoubleBladeLAnimationPlayer.Play("BladeLAttack");
	});
	}
	
}
