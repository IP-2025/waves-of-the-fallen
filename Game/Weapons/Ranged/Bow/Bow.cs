using Godot;
using System;

public partial class Bow : RangedWeapon
{
	private const string _resBase = "res://Weapons/Ranged/Bow/";
	private const string _resourcePath = _resBase + "Resources/";
	private const string _projectilePath = _resBase + "bow_arrow.tscn";


	public override string ResourcePath => _resBase + "Resources/";
	public override string IconPath => _resourcePath + "BowEmpty.png";
	public override float DefaultRange { get; set; } = 500f;
	public override int DefaultDamage { get; set; } = BowArrow.DefaultDamage;
	public override int DefaultPiercing { get; set; } = BowArrow.DefaultPiercing;
	public override float DefaultSpeed { get; set; } = BowArrow.DefaultSpeed;
	public override float ShootDelay{ get; set; } = 1.2f;
	public override int SoundFrame => 2;
	
	private float _shootCooldown;
	private float _timeUntilShoot;
	
	
	private static readonly PackedScene _arrowPacked = GD.Load<PackedScene>(_projectilePath);
	

	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/BowSprite");
		projectileScene = _arrowPacked;

		_shootCooldown   = 1f/ShootDelay;
		_timeUntilShoot  = _shootCooldown;
	}
	
	public override void _Process(double delta)
	{
		// Laufenden Countdown aktualisieren
		_timeUntilShoot -= (float)delta;

		if (_timeUntilShoot <= 0f)
		{
			// Zeit ist abgelaufen → schießen
			OnTimerTimeoutBow();
			_timeUntilShoot = _shootCooldown;
		}
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

	public void _on_bow_sprite_frame_changed()
	{
		if (SoundFrame == GetNode<AnimatedSprite2D>("WeaponPivot/BowSprite").Frame)
		{
			SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("bowFires"), GlobalPosition);
		}
	}
	
	public void SetNewStats(float newDelay)
	{
		ShootDelay     = newDelay;
		_shootCooldown  = newDelay;
	}
}
