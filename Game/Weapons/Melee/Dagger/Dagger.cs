using Godot;
using System;

public partial class Dagger : MeleeWeapon
{
	private AnimationPlayer DaggerAnimationPlayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DaggerAnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		MeleeDamage = 200;
		WeaponRange = 50;
		Speed = 0.1f;
	}

	public async void OnTimerTimeoutDagger()
	{ 
		ShootMeleeVisual(() =>
	{
		DaggerAnimationPlayer.Play("stab");
	});
		await ToSignal(GetTree().CreateTimer(Speed), "timeout");
	}
}
