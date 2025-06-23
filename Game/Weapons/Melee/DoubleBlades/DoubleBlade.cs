using System;
using Godot;

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
	public override float DefaultRange { get; set; } = 100f;
	public override int DefaultDamage { get; set; } = 100;
	
	public override float ShootDelay{ get; set; } = 1.6f;
	
	private float _shootCooldown;
	private float _timeUntilShoot;

	public override void _Ready()
    {
        GD.Print("Start SwordAnimation");
        leftBlade = GetNode<DoubleBladeL>("DoubleBladeL");
        rightBlade = GetNode<DoubleBladeR>("DoubleBladeR");

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

		DefaultDamage += (int)(dex + str + @int / 4.5f) / 3;
		ShootDelay *= Math.Max(Math.Min(1f / Math.Max((dex - 80) / 50f, 1), 1f), 0.5f);
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
