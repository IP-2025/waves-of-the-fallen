using Godot;
using System;

public partial class DoubleBlade : MeleeWeapon
{
	private DoubleBladeL leftBlade;
	private DoubleBladeR rightBlade;
	private bool useRightNext = true;
	
	private const string _resBase = "res://Weapons/Melee/DoubleBlades/";

	public override int DefaultPiercing { get; set; } = 0;
	public override float DefaultSpeed { get; set; } = 0f;
	public override string ResourcePath => _resBase + "Resources/";
	public override string IconPath => ResourcePath + "DoubleBlades.png";
	public override float DefaultRange { get; set; } = 120f;
	public override int DefaultDamage { get; set; } = 150;
	
	public override float ShootDelay{ get; set; } = 1f;
	
	private float _shootCooldown;
	private float _timeUntilShoot;

	public override void _Ready()
	{
		GD.Print("Start SwordAnimation");
		leftBlade = GetNode<DoubleBladeL>("DoubleBladeL");
		rightBlade = GetNode<DoubleBladeR>("DoubleBladeR");
		_shootCooldown   = 1f/ShootDelay;
		_timeUntilShoot  = _shootCooldown;
	}
	
	public override void _Process(double delta)
	{
		_timeUntilShoot -= (float)delta;

		if (_timeUntilShoot <= 0f)
		{
			OnTimeTimeout();
			_timeUntilShoot = _shootCooldown;
		}
	}

	private async void OnTimeTimeout()
	{
		if (useRightNext)
		{
			rightBlade?.StartAttack();
		}
		else
		{
			leftBlade?.StartAttack();
		}
		useRightNext = !useRightNext;
		await ToSignal(GetTree().CreateTimer(0.1), "timeout");
	}
}
