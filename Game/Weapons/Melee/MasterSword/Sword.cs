using Godot;
using System;
public partial class Sword : MeleeWeapon
{
	private AnimationPlayer SwordAnimationPlayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SwordAnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		MeleeDamage = 100;
		WeaponRange = 100;
	}

	private async void OnTimerTimeoutAttack()
	{ 
		ShootMeleeVisual(() =>
	{
		//GD.Print("Start SwordAnimation");
		SwordAnimationPlayer.Play("SwordAttack");
	});
		await ToSignal(GetTree().CreateTimer(0.2), "timeout");
	}
	
}
