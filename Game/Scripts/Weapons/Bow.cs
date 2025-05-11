using Godot;
using System;

public partial class Bow : RangedWeapon
{
	private PackedScene arrowScene = GD.Load<PackedScene>("res://Scenes/Weapons/bow_arrow.tscn");
	
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/BowSprite");
		projectileScene = arrowScene;
		WeaponRange = 500f;

	}
	
	public async void OnTimerTimeoutBow()
	{
		var target = FindNearestEnemy();
		if (target == null)
			return;

		animatedSprite.Play("shoot");
		await ToSignal(GetTree().CreateTimer(0.13), "timeout");

		Shoot();
	}
	
}
