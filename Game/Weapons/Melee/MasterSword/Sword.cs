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
		Speed = 0.1f;
		SwordTrailTest.Visible = false;
	}

	private async void OnTimerTimeoutAttack()
	{ 
		ShootMeleeVisual(() =>
	{
		SwordAnimationPlayer.Play("SwordAttack");
		SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("swordSwings"), GlobalPosition, -5);
	});
		await ToSignal(GetTree().CreateTimer(Speed), "timeout");
	}
}
