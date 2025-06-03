using Godot;
using System;

public partial class WarHammer : RangedWeapon
{
	private PackedScene throwHammerScene = GD.Load<PackedScene>("res://Weapons/Ranged/WarHammer/hammerProjectile.tscn");
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/WarHammer");
		projectileScene = throwHammerScene;
		WeaponRange = 300f;
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	private async void OnTimerTimeoutAttackHammer()
	{
		var target = FindNearestEnemy();
		if (target == null)
			return;

		//animatedSprite.Play("Crush");
		await ToSignal(GetTree().CreateTimer(1), "timeout");
		Shoot();
		SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("warhammerThrow"), GlobalPosition);
	}
}
