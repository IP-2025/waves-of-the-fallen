using Godot;

[GlobalClass]
public partial class Lightningstaff : RangedWeapon
{
	private const string _resBase = "res://Weapons/Ranged/MagicStaffs/Lightningstaff/";
	private const string _resourcePath = _resBase + "Resources/";
	private const string _iconPath = _resourcePath + "LightningStaff.png";
	private const string _projectilePath = _resBase + "lightning.tscn";
	
	public override string ResourcePath => _resourcePath;
	public override string IconPath => _iconPath;
	public override float DefaultRange { get; set; } = 400f;
	public override int DefaultDamage { get; set; } = Lightning.DefaultDamage;
	public override int DefaultPiercing { get; set; } = Lightning.DefaultPiercing;
	public override float DefaultSpeed { get; set; } = Lightning.DefaultSpeed;
	public override float ShootDelay { get; set; } = 0.8f;
	public override int SoundFrame => 3;
	
	private float _shootCooldown;
	private float _timeUntilShoot;

	private static readonly PackedScene _lightningPacked = GD.Load<PackedScene>(_projectilePath);
	

	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/LightningStaffSprite");
		projectileScene = _lightningPacked;
		
		_shootCooldown  = 1f / ShootDelay;
		_timeUntilShoot = _shootCooldown;
	}
	
	public override void _Process(double delta)
	{
		// Countdown verringern
		_timeUntilShoot -= (float)delta;

		if (_timeUntilShoot <= 0f)
		{
			OnTimerTimeoutLightningstaff();
			_timeUntilShoot = _shootCooldown;
		}
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

	public void _on_lightningstaff_sprite_frame_changed()
	{
		if (SoundFrame == animatedSprite.Frame)
		{
			SoundManager.Instance.PlaySoundAtPosition(
				SoundManager.Instance.GetNode<AudioStreamPlayer2D>("lightningstaffFires"),
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
