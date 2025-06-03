using Godot;

[GlobalClass]
public partial class FireStaff : RangedWeapon
{
	private const string _resBase = "res://Weapons/Ranged/MagicStaffs/Firestaff/";
	private const string _resourcePath = _resBase + "Resources/";
	private const string _iconPath = _resourcePath + "FireStaff.png";
	private const string _projectilePath = _resBase + "fireball.tscn";

	public override string ResourcePath => _resourcePath;
	public override string IconPath => _iconPath;
	public override float DefaultRange { get; set; } = 500f;
	public override int DefaultDamage { get; set; } = FireBall.DefaultDamage;
	public override int DefaultPiercing { get; set; } = FireBall.DefaultPiercing;
	public override float DefaultSpeed { get; set; } = FireBall.DefaultSpeed;
	public override float ShootDelay { get; set; } = 0.5f;
	public override int SoundFrame => 5;

	private static readonly PackedScene _fireballPacked = GD.Load<PackedScene>(_projectilePath);

	private float _shootCooldown;
	private float _timeUntilShoot;

	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/FireStaffSprite");
		projectileScene = _fireballPacked;
		
		_shootCooldown  = 1f / ShootDelay;
		_timeUntilShoot = _shootCooldown;
	}
	
	public override void _Process(double delta)
	{
		// Countdown verringern
		_timeUntilShoot -= (float)delta;

		if (_timeUntilShoot <= 0f)
		{
			OnTimerTimeoutFireStaff();
			_timeUntilShoot = _shootCooldown;
		}
	}

	public async void OnTimerTimeoutFireStaff()
	{
		var target = FindNearestEnemy();
		if (target == null)
			return;
		animatedSprite.Play("shoot");
		await ToSignal(GetTree().CreateTimer(1.7), "timeout");
		Shoot();
	}

	public void _on_firestaff_sprite_frame_changed()
	{
		if (SoundFrame == animatedSprite.Frame)
		{
			SoundManager.Instance.PlaySoundAtPosition(
				SoundManager.Instance.GetNode<AudioStreamPlayer2D>("firestaffFires"),
				GlobalPosition
			);
		}
	}
	
	public void SetNewStats(float newDelay)
	{
		ShootDelay     = newDelay;
		_shootCooldown = 1f / newDelay;
	}
}
