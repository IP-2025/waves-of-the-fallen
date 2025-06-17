using Godot;
using System;
using System.Collections.Generic;

public partial class Healstaff : Weapon
{
	protected AnimatedSprite2D animatedSprite;
	protected AnimatedSprite2D healArea;
	
	private const string _resBase = "res://Weapons/Ranged/MagicStaffs/Healsftaff/";
	private const string _resourcePath = _resBase + "Resources/";
	
	public override int DefaultPiercing { get; set; } = 0;
	public override float DefaultSpeed { get; set; } = 0f;

	public string ResourcePath => _resourcePath;
	public override string IconPath => _resourcePath + "HealStaff1.png";
	public override float DefaultRange { get; set; } = 170f;
	public override int DefaultDamage { get; set; } = 50;
	public override float ShootDelay { get; set; } = 0.1f;
	public int SoundFrame => 3;
	
	private float _shootCooldown;
	private float _timeUntilShoot;
	private int _staffFiresFrame = 4;

	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/HealStaffSprite");
		healArea = GetNode<AnimatedSprite2D>("./Healcircle");

		int dexdummy = 100;
		int strdummy = 100;
		int intdummy = 100;

		DefaultDamage += (int)(dexdummy / 3 + strdummy / 3 + intdummy) / 30;
		ShootDelay *= Math.Max(Math.Min(1f / Math.Max((dexdummy-80) / 50f, 1), 1f), 0.5f);
		DefaultRange = DefaultRange + Math.Max(Math.Min((intdummy-100f)/2f,50f),0);

		_shootCooldown = ShootDelay;
		_timeUntilShoot = _shootCooldown;
	}
	
	public override void _Process(double delta)
	{
		// Countdown verringern
		_timeUntilShoot -= (float)delta;

		if (_timeUntilShoot <= 0f)
		{
			OnTimerTimeoutHealstaff();
			_timeUntilShoot = _shootCooldown;
		}
	}
	
	protected List<Node> FindNearPlayers()
	{
		List<Node> nearPlayers = new List<Node>();
		foreach (Node node in GetTree().GetNodesInGroup("player"))
		{
			
			if (node is not DefaultPlayer playerNode)
				continue;

			float dist = GlobalPosition.DistanceTo(playerNode.GlobalPosition);

			if (dist <= DefaultRange)
			{
				nearPlayers.Add(playerNode);
			}
		}
		return nearPlayers;
	}
	
	public async void OnTimerTimeoutHealstaff()
	{
		healArea.Visible = true;
		animatedSprite.Play("shoot");
		healArea.Play("heal");
		await ToSignal(GetTree().CreateTimer(2.8), "timeout");
		Heal();
		healArea.Visible = false;
	}

	protected void Heal()
	{
		List<Node> nearPlayers = FindNearPlayers();
		foreach (Node node in nearPlayers)
		{
			var healthNode = node.GetNodeOrNull<Health>("Health");
			if (healthNode != null)
			{
				healthNode.Heal(DefaultDamage);
			}
		}
	}
	
	public void _on_heal_staff_sprite_frame_changed() {
		if(_staffFiresFrame == GetNode<AnimatedSprite2D>("WeaponPivot/HealStaffSprite").Frame) {
			SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("healstaffFires"), GlobalPosition);
		}
	}
	
	public void SetNewStats(float newDelay)
	{
		ShootDelay     = newDelay;
		_shootCooldown = 1f / newDelay;
	}
}
