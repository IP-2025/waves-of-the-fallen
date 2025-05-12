using Godot;
using System;

public partial class FireStaff : RangedWeapon
{
	private PackedScene arrowScene = GD.Load<PackedScene>("res://Scenes/Weapons/bow_arrow.tscn");
	
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/FireStaffSprite");
		projectileScene = arrowScene;
		WeaponRange = 600f;

	}
	
	public async void OnTimerTimeoutIFirestaff()
	{
		var target = FindNearestEnemy();
		if (target == null)
			return;

		animatedSprite.Play("shoot");
		await ToSignal(GetTree().CreateTimer(0.4), "timeout");
		Shoot();
	}
	
}
