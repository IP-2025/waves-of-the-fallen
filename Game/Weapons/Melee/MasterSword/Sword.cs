using Godot;
using System;
public partial class Sword : MeleeWeapon
{
	private AnimationPlayer SwordAnimationPlayer;
	private Sprite2D SwordTrailTest;
	private const string _resBase = "res://Weapons/Melee/MasterSword/";
	private const string _resourcePath = _resBase + "Resources/";
	
	public override int DefaultPiercing { get; set; } = 0;
	public override float DefaultSpeed { get; set; } = 0f;
	
	public override string ResourcePath => _resBase + "Resources/";
	public override string IconPath => _resourcePath + "MasterSword1.png";
	public override float DefaultRange { get; set; } = 140f;
	public override int DefaultDamage { get; set; } = 100;
	
	public override float ShootDelay{ get; set; } = 1.2f;
	
	private float _shootCooldown;
	private float _timeUntilShoot;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SwordAnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		SwordTrailTest = GetNode<Sprite2D>("SwordTrailTest");
		_shootCooldown   = 1f/ShootDelay;
		_timeUntilShoot  = _shootCooldown;
		SwordTrailTest.Visible = false;
	}
	
	public override void _Process(double delta)
	{
		// Laufenden Countdown aktualisieren
		_timeUntilShoot -= (float)delta;

		if (_timeUntilShoot <= 0f)
		{
			// Zeit ist abgelaufen → schießen
			OnTimerTimeoutAttack();
			_timeUntilShoot = _shootCooldown;
		}
	}
	
	private async void OnTimerTimeoutAttack()
	{ 
		ShootMeleeVisual(() =>
	{
		SwordAnimationPlayer.Play("SwordAttack");
		SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("swordSwings"), GlobalPosition, -5);
	});
		await ToSignal(GetTree().CreateTimer(0.2), "timeout");
	}
	
	public void SetNewStats(float newDelay)
	{
		ShootDelay     = newDelay;
		_shootCooldown  = newDelay;
	}
}
