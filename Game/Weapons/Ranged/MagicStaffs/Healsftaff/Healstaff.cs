using Godot;
using System;
using System.Collections.Generic;

public partial class Healstaff : Area2D
{
	protected AnimatedSprite2D animatedSprite;
	protected AnimatedSprite2D healArea;
	
	private const string _resBase = "res://Weapons/Ranged/MagicStaffs/Healsftaff/";
	private const string _resourcePath = _resBase + "Resources/";

	public string ResourcePath => _resourcePath;
	public string IconPath => _resourcePath + "HealStaff1.png";
	public float DefaultRange { get; set; } = 220f;
	public int DefaultHeal { get; set; } = 50;
	public float ShootDelay { get; set; } = 0.1f;
	public int SoundFrame => 3;
	
	private float _shootCooldown;
	private float _timeUntilShoot;
	private int _staffFiresFrame = 4;
	
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/HealStaffSprite");
		healArea = GetNode<AnimatedSprite2D>("./Healcircle");

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
				healthNode.Heal(DefaultHeal);
			}
		}
	}
	
	public void _on_heal_staff_sprite_frame_changed() {
		if(_staffFiresFrame == GetNode<AnimatedSprite2D>("WeaponPivot/HealStaffSprite").Frame) {
			SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("healstaffFires"), GlobalPosition);
		}
	}
}
