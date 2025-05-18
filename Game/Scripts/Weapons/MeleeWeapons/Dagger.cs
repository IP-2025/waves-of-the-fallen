using Godot;
using System;

public partial class Dagger : MeleeWeapon
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/Stab");
		MeleeDamage = 50;
	}

	public async void OnTimerTimeoutDagger()
	{
		animatedSprite.Play("Stab");
		await ToSignal(GetTree().CreateTimer(0.13), "timeout");
		//MeleeAttack(target);
	}	
}
