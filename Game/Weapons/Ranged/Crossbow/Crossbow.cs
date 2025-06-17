using System;
using Godot;


[GlobalClass]
public partial class Crossbow : RangedWeapon
{
	private const string _resBase = "res://Weapons/Ranged/Crossbow/";
	private const string _resourcePath = _resBase + "Resources/";
	private const string _projectilePath = _resBase + "crossbow_arrow.tscn";

	public override string ResourcePath => _resourcePath;
	public override string IconPath => _resourcePath + "CrossbowEmpty.png";
	public override float DefaultRange { get; set; } = 700f;
	public override int DefaultDamage { get; set; } = CrossbowArrow.DefaultDamage;
	public override int DefaultPiercing { get; set; } = CrossbowArrow.DefaultPiercing;
	public override float DefaultSpeed { get; set; } = CrossbowArrow.DefaultSpeed;
	public override float ShootDelay { get; set; } = 0.5f;
	public override int SoundFrame => 3;
	
	private float _shootCooldown;
	private float _timeUntilShoot;

	private static readonly PackedScene _arrowPacked = GD.Load<PackedScene>(_projectilePath);

	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/CrossbowSprite");
		projectileScene = _arrowPacked;

		int dexdummy = 100;
		int strdummy = 100;
		int intdummy = 100;

		DefaultDamage += (int)(dexdummy / 1.2f + strdummy + intdummy / 5) / 3;
		ShootDelay *= Math.Max(Math.Min(1f / Math.Max((strdummy-80) / 50f, 1), 1f), 0.1f);

		_shootCooldown = ShootDelay;
		_timeUntilShoot = _shootCooldown;
	}

	public override void _Process(double delta)
	{
		// Countdown verringern
		_timeUntilShoot -= (float)delta;

		if (_timeUntilShoot <= 0f)
		{
			OnTimerTimeoutCrossbow();
			_timeUntilShoot = _shootCooldown;
		}
	}
	
	public async void OnTimerTimeoutCrossbow()
	{
		var target = FindNearestEnemy();
		if (target == null)
			return;

		animatedSprite.Play("shoot");
		await ToSignal(GetTree().CreateTimer(0.2), "timeout");
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
	
	public void SetNewStats(float newDelay)
	{
		ShootDelay     = newDelay;
		_shootCooldown = 1f / newDelay;
	}
}
