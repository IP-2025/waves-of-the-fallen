using System;
using Godot;


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
	public override float ShootDelay{ get; set; } = 1.5f;
	public override int SoundFrame => 2;
	
	private float _shootCooldown;
	private float _timeUntilShoot;
	
	
	private static readonly PackedScene _arrowPacked = GD.Load<PackedScene>(_projectilePath);


	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/BowSprite");
		projectileScene = _arrowPacked;

		calculateWeaponStats();

		_shootCooldown = ShootDelay;
		_timeUntilShoot = _shootCooldown;
	}

	private void calculateWeaponStats()
	{
		DefaultPlayer OwnerNode = GetNode("../../").GetParentOrNull<DefaultPlayer>();
		dex = OwnerNode.Dexterity;
		str = OwnerNode.Strength;
		@int = OwnerNode.Intelligence;
		DefaultDamage = DefaultDamage + (int)(dex + str / 3.5f + @int / 7) / 3;
		ShootDelay *= Math.Max(Math.Min(1f / Math.Max((dex - 80f) / 50f, 1f), 1f), 0.5f);
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
