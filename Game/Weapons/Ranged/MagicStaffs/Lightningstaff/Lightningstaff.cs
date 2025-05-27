using Godot;

[GlobalClass]
public partial class Lightningstaff : RangedWeapon
{
	private const string _resBase        = "res://Weapons/Ranged/MagicStaffs/Lightningstaff/";
	private const string _resourcePath   = _resBase + "Resources/";
	private const string _iconPath       = _resourcePath + "LightningStaff.png";
	private const string _projectilePath = _resBase + "lightning.tscn";

	private const float _defaultRange = 600f;
	private const float _shootDelay   = 0.9f;
	private const int   _soundFrame   = 3;

	public override string ResourcePath    => _resourcePath;
	public override string IconPath        => _iconPath;
	public override float  DefaultRange    => _defaultRange;
	public override int    DefaultDamage   => Lightning.DefaultDamage;
	public override int    DefaultPiercing => Lightning.DefaultPiercing;
	public override float  DefaultSpeed    => Lightning.DefaultSpeed;
	public override float  ShootDelay      => _shootDelay;
	public override int    SoundFrame      => _soundFrame;

	private static readonly PackedScene _lightningPacked = GD.Load<PackedScene>(_projectilePath);

	private AnimatedSprite2D animatedSprite;

public override void _Ready()
	{
		animatedSprite  = GetNode<AnimatedSprite2D>("./WeaponPivot/LightningStaffSprite");
		projectileScene = _lightningPacked;
		WeaponRange     = DefaultRange;
		GetNode<Timer>("Timer").WaitTime = ShootDelay;
		GetNode<Timer>("Timer").Timeout += OnTimerTimeoutLightningstaff;
	}

	public async void OnTimerTimeoutLightningstaff()
	{
		GD.Print("f");
		var target = FindNearestEnemy();
		if (target == null)
			return;
		animatedSprite.Play("shoot");
		Shoot();
	}

	public void _on_lightningstaff_sprite_frame_changed()
	{
		if (SoundFrame == animatedSprite.Frame)
		{
			SoundManager.Instance.PlaySoundAtPosition(
				SoundManager.Instance.GetNode<AudioStreamPlayer2D>("lightningCasts"),
				GlobalPosition
			);
		}
	}
}
