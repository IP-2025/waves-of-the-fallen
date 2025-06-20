using System;
using Godot;

[GlobalClass]
public partial class Kunai : RangedWeapon
{
	private const string _resBase = "res://Weapons/Ranged/Kunai/";
	private const string _resourcePath = _resBase + "Resources/";
	private const string _projectilePath = _resBase + "kunai_projectile.tscn";

	public override string ResourcePath => _resourcePath;
	public override string IconPath => _resourcePath + "Kunai.png";
	public override float DefaultRange { get; set; } = 300f;
	public override int DefaultDamage { get; set; } = KunaiProjectile.DefaultDamage;
	public override int DefaultPiercing { get; set; } = KunaiProjectile.DefaultPiercing;
	public override float DefaultSpeed { get; set; } = KunaiProjectile.DefaultSpeed;

	public override float ShootDelay { get; set; } = 0.4f;
	public override int SoundFrame => 1;
	
	private float _shootCooldown;
	private float _timeUntilShoot;

	private static readonly PackedScene _kunaiPacked = GD.Load<PackedScene>(_projectilePath);

	public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/KunaiSprite");
        projectileScene = _kunaiPacked;

        _CalculateWeaponStats();

        _shootCooldown = ShootDelay;
        _timeUntilShoot = _shootCooldown;
    }

    private void _CalculateWeaponStats()
    {
		DefaultPlayer OwnerNode = GetNode("../../").GetParentOrNull<DefaultPlayer>();
        int dex = OwnerNode.Dexterity;
        int str = OwnerNode.Strength;
        int @int = OwnerNode.Intelligence;

        DefaultDamage += (int)(dex + str / 8 + @int / 8) / 3;
        ShootDelay *= Math.Max(Math.Min(1f / Math.Max((dex - 80) / 50f, 1), 1f), 0.2f);
    }

    public override void _Process(double delta)
	{
		// Countdown verringern
		_timeUntilShoot -= (float)delta;

		if (_timeUntilShoot <= 0f)
		{
			OnTimerTimeoutKunai();
			_timeUntilShoot = _shootCooldown;
		}
	}
	
	public async void OnTimerTimeoutKunai()
	{
		var target = FindNearestEnemy();
		if (target == null)
			return;

		animatedSprite.Play("shoot");
		Shoot();
	}

	public void _on_kunai_sprite_frame_changed()
	{
		if (SoundFrame == animatedSprite.Frame)
		{
			SoundManager.Instance
				.PlaySoundAtPosition(
					SoundManager.Instance.GetNode<AudioStreamPlayer2D>("kunaiThrows"),
					GlobalPosition,
					-6.9f
				);
		}
	}
	
	public void SetNewStats(float newDelay)
	{
		ShootDelay     = newDelay;
		_shootCooldown = 1f / newDelay;
	}
}
