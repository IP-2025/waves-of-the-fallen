using Godot;
using System;

public partial class Dagger : MeleeWeapon
{
	[Export] public int Damage = 50;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Standard Dagger Animation or use nothing.
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	protected override void MeleeAttack(Node target)
	{
		//_animatedSprite?.Play("attack");
		if (target.HasMethod("TakeDamage"))
		{
			target.Call("TakeDamage", Damage);
		}
	}
	
}
