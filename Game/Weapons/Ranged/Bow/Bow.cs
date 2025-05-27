using Godot;
using System;

public partial class Bow : RangedWeapon
{
	private const string _resBase = "res://Weapons/Ranged/Bow/";
	private const string _resourcePath = _resBase + "Resources/";
	private const string _projectilePath = _resBase + "bow_arrow.tscn";


	public override string ResourcePath => _resBase + "Resources/";
	public override string IconPath => _resourcePath + "BowEmpty.png";
	public override float DefaultRange => 500f;
	public override int DefaultDamage => BowArrow.DefaultDamage;
	public override int DefaultPiercing => BowArrow.DefaultPiercing;
	public override float DefaultSpeed => BowArrow.DefaultSpeed;
	public override float ShootDelay => 6.03f;
	public override int SoundFrame => 2;


	private static readonly PackedScene _arrowPacked = GD.Load<PackedScene>(_projectilePath);

	private AnimatedSprite2D animatedSprite;

	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/BowSprite");
		projectileScene = _arrowPacked;
		WeaponRange = DefaultRange;

		GetNode<Timer>("Timer").WaitTime = ShootDelay;
	}

	public async void OnTimerTimeoutBow()
	{
		var target = FindNearestEnemy();
		if (target == null)
			return;

		animatedSprite.Play("shoot");
		Shoot();
	}

	public void _on_bow_sprite_frame_changed()
	{
		if (SoundFrame == GetNode<AnimatedSprite2D>("WeaponPivot/BowSprite").Frame)
		{
			SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("bowFires"), GlobalPosition);
		}
	}
}
