using Godot;
using System;
public partial class DoubleBladeL : MeleeWeapon
{
	private AnimationPlayer DoubleBladeLAnimationPlayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DoubleBladeLAnimationPlayer = GetNode<AnimationPlayer>("DoubleBladeL/AnimationPlayerL");
		MeleeDamage = 100;
		WeaponRange = 100;
	}

	public void StartAttack()
	{ 
		ShootMeleeVisual(() =>
	{
		DoubleBladeLAnimationPlayer.Play("BladeLAttack");
	});
	}
	
}
