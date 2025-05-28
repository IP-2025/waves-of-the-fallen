using Godot;
using System;

public partial class Dagger : MeleeWeapon
{
	// Called when the node enters the scene tree for the first time.
	private const string _resBase = "res://Weapons/Melee/Dagger/";
	private const string _resourcePath = _resBase + "Resources/";

	public override int DefaultPiercing { get; set; } = 0;
	public override float DefaultSpeed { get; set; } = 0f;
	public override string ResourcePath => _resBase + "Resources/";
	public override string IconPath => _resourcePath + "Dagger.png";
	public override float DefaultRange { get; set; } = 100f;
	public override int DefaultDamage { get; set; } = 50;
	
	public override float ShootDelay{ get; set; } = 1f;
	
	private float _shootCooldown;
	private float _timeUntilShoot;
	
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/Stab");
		
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
			OnTimerTimeoutDagger();
			_timeUntilShoot = _shootCooldown;
		}
	}

	public async void OnTimerTimeoutDagger()
	{
		ShootMeleeVisual(() =>
		{
			animatedSprite.Play("stab");
		});
		
		await ToSignal(GetTree().CreateTimer(0.13), "timeout");
		//MeleeAttack(target);
	}	
	
	public void SetNewStats(float newDelay)
	{
		ShootDelay     = newDelay;
		_shootCooldown  = newDelay;
	}
}
