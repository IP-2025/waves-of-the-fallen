using Godot;
using System;

public partial class Kunai : RangedWeapon
{
	private PackedScene kunai_projectile_Scene = GD.Load<PackedScene>("res://Scenes/Weapons/kunai_projectile.tscn");
	
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/KunaiSprite");
		projectileScene = kunai_projectile_Scene;
		WeaponRange = 300f;

	}
	
	
	public async void OnTimerTimeoutKunai()
	{
		var target = FindNearestEnemy();
		if (target == null)
			return;

		animatedSprite.Play("shoot");
		Shoot();
	}
}
