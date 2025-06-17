using System;
using Godot;

public partial class WarHammer : RangedWeapon
{
	private const string _resBase = "res://Weapons/Ranged/WarHammer/";
	private const string _resourcePath = _resBase + "Resources/";
	private const string _projectilePath = _resBase + "hammerProjectile.tscn";


	public override string ResourcePath => _resBase + "Resources/";
	public override string IconPath => _resourcePath + "WarHammer.png";
	public override float DefaultRange { get; set; } = 300f;
	public override int DefaultDamage { get; set; } = HammerProjectile.DefaultDamage;
	public override int DefaultPiercing { get; set; } = HammerProjectile.DefaultPiercing;
	public override float DefaultSpeed { get; set; } = HammerProjectile.DefaultSpeed;
	public override float ShootDelay { get; set; } = 1f;
	public override int SoundFrame => 2;

	private float _shootCooldown;
	private float _timeUntilShoot;
	
	private static readonly PackedScene _throwHammerScene = GD.Load<PackedScene>(_projectilePath);

	private AnimatedSprite2D animatedSprite;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/WarHammer");
		projectileScene = _throwHammerScene;

		_shootCooldown = 1f / ShootDelay;
		_timeUntilShoot = _shootCooldown;

		int dexdummy = 100;
		int strdummy = 100;
		int intdummy = 100;

		DefaultDamage += (int)(dexdummy / 5 + strdummy + intdummy / 7.5f) / 3;
		ShootDelay *= Math.Max(Math.Min(1f / Math.Max((strdummy-80) / 50f, 1), 1f), 0.1f);
	}
	
	public override void _Process(double delta)
	{
		// Countdown verringern
		_timeUntilShoot -= (float)delta;

		if (_timeUntilShoot <= 0f)
		{
			OnTimerTimeoutAttackHammer();
			_timeUntilShoot = _shootCooldown;
		}
	}
	
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
	
	public void SetNewStats(float newDelay)
	{
		ShootDelay     = newDelay;
		_shootCooldown = 1f / newDelay;
	}
}
