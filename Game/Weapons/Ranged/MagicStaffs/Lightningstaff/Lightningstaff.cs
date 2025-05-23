using Godot;
using System;

public partial class Lightningstaff : RangedWeapon
{
	private PackedScene lightningScene = GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Lightningstaff/lightning.tscn");
	private int staffFiresFrame = 3;
	
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/LightningStaffSprite");
		projectileScene = lightningScene;
		WeaponRange = 600f;
		

	}
	
	public async void OnTimerTimeoutLightningstaff()
	{
		var target = FindNearestEnemy();
		if (target == null)
			return;

		animatedSprite.Play("shoot");
		await ToSignal(GetTree().CreateTimer(0.9), "timeout");
		Shoot();
	}
	
	public void _on_lightning_staff_sprite_frame_changed() {
		if(staffFiresFrame == GetNode<AnimatedSprite2D>("WeaponPivot/LightningStaffSprite").Frame) {
			SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("lightningstaffFires"), GlobalPosition);
		}
	}
}
