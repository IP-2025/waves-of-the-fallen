using Godot;
using System;

public partial class Dagger : MeleeWeapon
{
	private PackedScene daggerScene = GD.Load<PackedScene>("res://Scenes/Weapons/dagger.tscn");
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/Stab");
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
	public override void _Process(double delta)
	{
		animatedSprite.Play("Stab");
	}
	
}
