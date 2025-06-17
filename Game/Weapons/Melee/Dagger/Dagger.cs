using System;
using Godot;

public partial class Dagger : MeleeWeapon
{
	private AnimationPlayer DaggerAnimationPlayer;
	// Called when the node enters the scene tree for the first time.
	private const string _resBase = "res://Weapons/Melee/Dagger/";
	private const string _resourcePath = _resBase + "Resources/";

	public override int DefaultPiercing { get; set; } = 0;
	public override float DefaultSpeed { get; set; } = 0f;
	public override string ResourcePath => _resBase + "Resources/";
	public override string IconPath => _resourcePath + "Dagger.png";
	public override float DefaultRange { get; set; } = 100f;
	public override int DefaultDamage { get; set; } = 80;
	public override float ShootDelay{ get; set; } = 1.3f;
	
	private float _shootCooldown;
	private float _timeUntilShoot;

	public override void _Ready()
	{
		DaggerAnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/Stab");

		int dexdummy = 100;
		int strdummy = 100;
		int intdummy = 100;

		DefaultDamage = DefaultDamage + (dexdummy + strdummy / 3 + intdummy / 3) / 3;
		ShootDelay *= Math.Max(Math.Min(1f / Math.Max((dexdummy-80) / 50f, 1), 1f), 0.1f);
		DefaultRange = DefaultRange + Math.Max(Math.Min((dexdummy-100f)/2f,50f),0);

		_shootCooldown = ShootDelay;
		_timeUntilShoot = _shootCooldown;
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
			DaggerAnimationPlayer.Play("stab");
			SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("daggerStabs"), GlobalPosition, -5);
		});
		
		await ToSignal(GetTree().CreateTimer(0.2), "timeout");
		//MeleeAttack(target);
	}	
	
	public void SetNewStats(float newDelay)
	{
		ShootDelay     = newDelay;
		_shootCooldown  = newDelay;
	}
}
