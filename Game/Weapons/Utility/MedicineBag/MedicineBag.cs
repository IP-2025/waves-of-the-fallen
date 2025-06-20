using Godot;
using Game.Utilities.Multiplayer;
using System;

public partial class MedicineBag : Weapon
{
	private PackedScene medicine = GD.Load<PackedScene>("res://Weapons/Utility/MedicineBag/medicine.tscn");

	private AnimatedSprite2D medicineBagSprite;
	private int medicineBagFires = 4;

	private const string _resBase = "res://Weapons/Utility/MedicineBag/";
	private const string _resourcePath = _resBase + "Resources/";
	public override string IconPath => _resourcePath + "MedicineBag.png";

	public override float ShootDelay { get; set; } = 4f;
	public override float DefaultRange { get; set; } = 0;
	public override int DefaultDamage { get; set; } = Medicine.healValue;
	public override int DefaultPiercing { get; set; } = 0;
	public override float DefaultSpeed { get; set; } = 0;
	private float _shootCooldown;
	private float _timeUntilShoot;

	public override void _Ready()
	{
		medicineBagSprite = GetNode<AnimatedSprite2D>("MedicineBagSprite");

		int dexdummy = 100;
		int strdummy = 100;
		int intdummy = 100;

		DefaultDamage += (int)(dexdummy + strdummy / 3 + intdummy / 3) / 30;
		ShootDelay *= Math.Max(Math.Min(1f / Math.Max((dexdummy - 80) / 50f, 1), 1f), 0.5f);

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
			OnTimerTimeoutMedicineBag();
			_timeUntilShoot = _shootCooldown;
		}
	}

	public async void OnTimerTimeoutMedicineBag()
	{
		medicineBagSprite.Play();
		await ToSignal(GetTree().CreateTimer(1.2), "timeout");
		if (Multiplayer.IsServer() && GetNode<Health>("../../../Health").CurHealth > 0) // gets the healthnode of weaponowner, so medicine doesn't get thrown by server when dead
			Use();
	}

	protected void Use()
	{
		Area2D utilInstance = medicine.Instantiate() as Area2D;

		utilInstance.GlobalPosition = GlobalPosition;

		ulong id = utilInstance.GetInstanceId();
		utilInstance.Name = $"Medicine_{id}";
		Server.Instance.Entities[(long)id] = utilInstance;

		GetTree().CurrentScene.AddChild(utilInstance);
	}

	public void _on_medicine_bag_sprite_frame_changed()
	{
		if (medicineBagFires == GetNode<AnimatedSprite2D>("MedicineBagSprite").Frame)
		{
			SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("medicineBagThrow"), GlobalPosition, -10);
		}
	}
}
