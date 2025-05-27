using Godot;

[GlobalClass]
public partial class FireStaff : RangedWeapon
{
	private const string _resBase = "res://Weapons/Ranged/MagicStaffs/Firestaff/";
	private const string _resourcePath = _resBase + "Resources/";
	private const string _iconPath = _resourcePath + "FireStaff.png";
	private const string _projectilePath = _resBase + "fireball.tscn";

	private const float _defaultRange = 600f;
	private const float _shootDelay = 1.7f;
	private const int _soundFrame = 5;

	public override string ResourcePath => _resourcePath;
	public override string IconPath => _iconPath;
	public override float DefaultRange => _defaultRange;
	public override int DefaultDamage => FireBall.DefaultDamage;
	public override int DefaultPiercing => FireBall.DefaultPiercing;
	public override float DefaultSpeed => FireBall.DefaultSpeed;
	public override float ShootDelay => _shootDelay;
	public override int SoundFrame => _soundFrame;

	private static readonly PackedScene _fireballPacked = GD.Load<PackedScene>(_projectilePath);

	private AnimatedSprite2D animatedSprite;

	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/FireStaffSprite");
		projectileScene = _fireballPacked;
		WeaponRange = DefaultRange;
		GetNode<Timer>("Timer").WaitTime = ShootDelay;
	}

	public async void OnTimerTimeoutFireStaff()
	{
		var target = FindNearestEnemy();
		if (target == null)
			return;
		animatedSprite.Play("shoot");
		Shoot();
	}

	public void _on_firestaff_sprite_frame_changed()
	{
		if (SoundFrame == animatedSprite.Frame)
		{
			SoundManager.Instance.PlaySoundAtPosition(
				SoundManager.Instance.GetNode<AudioStreamPlayer2D>("fireStaffCasts"),
				GlobalPosition
			);
		}
	}
}
