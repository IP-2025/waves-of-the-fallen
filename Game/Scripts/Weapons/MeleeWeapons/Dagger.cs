using Godot;
using System;

public partial class Dagger : MeleeWeapon
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/Stab");
		MeleeDamage = 50;
		//Standard Dagger Animation or use nothing.
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	/*protected override void MeleeAttack(Node target)
	{
		animatedSprite.Play("stab");
		if (target.HasMethod("TakeDamage"))
		{
			target.Call("TakeDamage", Damage);
		}
	}*/
	public async void OnTimerTimeoutDagger()
	{
		animatedSprite.Play("Stab");
		await ToSignal(GetTree().CreateTimer(0.13), "timeout");
		//MeleeAttack(target);
	}	
}
