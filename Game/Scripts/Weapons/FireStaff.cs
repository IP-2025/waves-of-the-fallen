using Godot;
using System;

public partial class FireStaff : RangedWeapon
{
	private PackedScene fireBallScene = GD.Load<PackedScene>("res://Scenes/Weapons/fireball.tscn");
	private int staffFiresFrame = 3;
	
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
	
	public void _on_fire_staff_sprite_frame_changed() {
		if(staffFiresFrame == GetNode<AnimatedSprite2D>("WeaponPivot/FireStaffSprite").Frame) {
			SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("firestaffFires"), GlobalPosition);
		}
	}
}
