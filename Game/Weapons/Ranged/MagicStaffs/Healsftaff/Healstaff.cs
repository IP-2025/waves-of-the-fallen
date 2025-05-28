using Godot;
using System;
using System.Collections.Generic;

public partial class Healstaff : Area2D
{
	protected AnimatedSprite2D animatedSprite;
	protected AnimatedSprite2D healArea;
	protected float WeaponRange = 220f;
	protected int healValue = 50;
	private int staffFiresFrame = 4;
	
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

			if (dist <= WeaponRange)
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
				healthNode.Heal(healValue);
			}
		}
	}
	
	public void _on_heal_staff_sprite_frame_changed() {
		if(staffFiresFrame == GetNode<AnimatedSprite2D>("WeaponPivot/HealStaffSprite").Frame) {
			SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("healstaffFires"), GlobalPosition);
		}
	}
}
