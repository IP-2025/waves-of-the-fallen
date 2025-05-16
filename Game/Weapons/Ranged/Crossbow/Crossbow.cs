using Godot;
using System;
using System.Linq; // Needed for LINQ

public partial class Crossbow : RangedWeapon
{
	
	private PackedScene arrowScene = GD.Load<PackedScene>("res://Weapons/Ranged/Crossbow/crossbow_arrow.tscn");
	private int crossbowFiresFrame = 3;

	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/CrossbowSprite");
		projectileScene = arrowScene;
		WeaponRange = 700f;
	}
	
	public async void OnTimerTimeoutCrossbow()
	{
		animatedSprite.Play("shoot");
		await ToSignal(GetTree().CreateTimer(0.2), "timeout");
		
		Shoot();
	}
	
	public void _on_crossbow_sprite_frame_changed() {
		if(crossbowFiresFrame == GetNode<AnimatedSprite2D>("WeaponPivot/CrossbowSprite").Frame) {
			SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("crossbowFires"), GlobalPosition);
		}
	}
}
