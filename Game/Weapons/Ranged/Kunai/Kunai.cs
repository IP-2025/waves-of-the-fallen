using Godot;
using System;

public partial class Kunai : RangedWeapon
{
	private PackedScene kunai_projectile_Scene = GD.Load<PackedScene>("res://Weapons/Ranged/Kunai/kunai_projectile.tscn");
	
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/KunaiSprite");
		projectileScene = kunai_projectile_Scene;
		WeaponRange = 300f;

	}
	
	
	public void OnTimerTimeoutKunai()
	{
		var target = FindNearestEnemy();
		if (target == null)
			return;

		animatedSprite.Play("shoot");
		SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("kunaiThrows"), GlobalPosition, -6.9f);
		Shoot();
	}
}
