using Godot;
using System;

[GlobalClass]
public partial class Crossbow : RangedWeapon
{
	private const string _resBase        = "res://Weapons/Ranged/Crossbow/";
	private const string _resourcePath   = _resBase + "Resources/";
	private const string _projectilePath = _resBase + "crossbow_arrow.tscn";

	public override string ResourcePath    => _resourcePath;
	public override string IconPath        => _resourcePath + "CrossbowEmpty.png";
	public override float  DefaultRange    => 700f;
	public override int    DefaultDamage   => CrossbowArrow.DefaultDamage;
	public override int    DefaultPiercing => CrossbowArrow.DefaultPiercing;
	public override float  DefaultSpeed    => CrossbowArrow.DefaultSpeed;
	public override float  ShootDelay      => 0.20f;
	public override int    SoundFrame      => 3;

	private static readonly PackedScene _arrowPacked = GD.Load<PackedScene>(_projectilePath);
	private AnimatedSprite2D animatedSprite;

	public override void _Ready()
	{
		animatedSprite  = GetNode<AnimatedSprite2D>("./WeaponPivot/CrossbowSprite");
		projectileScene = _arrowPacked;
		WeaponRange     = DefaultRange;
		GetNode<Timer>("Timer").WaitTime = ShootDelay;
	}

	public async void OnTimerTimeoutCrossbow()
	{
		var target = FindNearestEnemy();
		if (target == null)
			return;

		animatedSprite.Play("shoot");
		Shoot();
	}

	public void _on_crossbow_sprite_frame_changed()
	{
		if (SoundFrame == GetNode<AnimatedSprite2D>("WeaponPivot/CrossbowSprite").Frame)
		{
			SoundManager.Instance
				.PlaySoundAtPosition(
					SoundManager.Instance.GetNode<AudioStreamPlayer2D>("crossbowFires"),
					GlobalPosition
				);
		}
	}
}
