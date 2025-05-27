using Godot;
using System;

[GlobalClass]
public partial class Kunai : RangedWeapon
{
	private const string _resBase        = "res://Weapons/Ranged/Kunai/";
	private const string _resourcePath   = _resBase + "Resources/";
	private const string _projectilePath = _resBase + "kunai_projectile.tscn";

	public override string ResourcePath    => _resourcePath;
	public override string IconPath        => _resourcePath + "Kunai.png";
	public override float  DefaultRange    => 300f;
	public override int    DefaultDamage   => KunaiProjectile.DefaultDamage;
	public override int    DefaultPiercing => KunaiProjectile.DefaultPiercing;
	public override float  DefaultSpeed    => KunaiProjectile.DefaultSpeed;

	public override float ShootDelay       => 0.05f;
	public override int   SoundFrame       => 0;

	private static readonly PackedScene _kunaiPacked = GD.Load<PackedScene>(_projectilePath);
	private AnimatedSprite2D animatedSprite;

	public override void _Ready()
	{
		animatedSprite  = GetNode<AnimatedSprite2D>("./WeaponPivot/KunaiSprite");
		projectileScene = _kunaiPacked;
		WeaponRange     = DefaultRange;
		GetNode<Timer>("Timer").WaitTime = ShootDelay;
	}

	public async void OnTimerTimeoutKunai()
	{
		var target = FindNearestEnemy();
		if (target == null)
			return;

		animatedSprite.Play("shoot");
		Shoot();
	}

	public void _on_kunai_sprite_frame_changed()
	{
		if (SoundFrame == animatedSprite.Frame)
		{
			SoundManager.Instance
				.PlaySoundAtPosition(
					SoundManager.Instance.GetNode<AudioStreamPlayer2D>("kunaiThrows"),
					GlobalPosition,
					-6.9f 
				);
		}
	}
}
