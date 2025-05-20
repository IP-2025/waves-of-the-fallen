using Godot;
using System;

public partial class Bow : RangedWeapon
{
	private PackedScene arrowScene = GD.Load<PackedScene>("res://Weapons/Ranged/Bow/bow_arrow.tscn");
	private int bowFiresFrame = 2;

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
	
	public void _on_bow_sprite_frame_changed() {
		if(bowFiresFrame == GetNode<AnimatedSprite2D>("WeaponPivot/BowSprite").Frame) {
			SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("bowFires"), GlobalPosition);
		}
	}
}
