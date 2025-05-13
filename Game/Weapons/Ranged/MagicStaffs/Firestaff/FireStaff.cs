using Godot;
using System;

public partial class FireStaff : RangedWeapon
{
	private PackedScene fireBallScene = GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Firestaff/fireball.tscn");
	
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/FireStaffSprite");
		projectileScene = fireBallScene;
		WeaponRange = 600f;

	}
	
	public async void OnTimerTimeoutIFirestaff()
	{
		var target = FindNearestEnemy();
		if (target == null)
			return;

		animatedSprite.Play("shoot");
		await ToSignal(GetTree().CreateTimer(1.7), "timeout");
		Shoot();
	}
	
}
