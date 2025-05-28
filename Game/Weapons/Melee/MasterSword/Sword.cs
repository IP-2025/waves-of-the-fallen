using Godot;
using System;
public partial class Sword : MeleeWeapon
{
	private AnimationPlayer SwordAnimationPlayer;
	private Sprite2D SwordTrailTest;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SwordAnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		SwordTrailTest = GetNode<Sprite2D>("SwordTrailTest");
		MeleeDamage = 100;
		WeaponRange = 100;
		SwordTrailTest.Visible = false;
	}

	private async void OnTimerTimeoutAttack()
	{ 
		ShootMeleeVisual(() =>
	{
		SwordAnimationPlayer.Play("SwordAttack");
	});
		await ToSignal(GetTree().CreateTimer(Speed), "timeout");
	}
}
