using Godot;
using System;
using System.Diagnostics;

public partial class BoostStrength : AbilityBase
{
	private Node2D _parent;
	private int tempStrength;
	private int boostAmount = 2;
	private int cooldown = 15;

	public override void _Ready()
	{
		_parent = GetParent<Node2D>();
		if (_parent is DefaultPlayer player)
		{
			tempStrength = player.Strength;
			player.Strength *= boostAmount;
			Debug.Print(player.Strength.ToString());
		}
	}

	private void _on_boost_strength_timer_timeout()
	{
		if (_parent is DefaultPlayer player)
		{
			player.Strength = tempStrength;
			Debug.Print(player.Strength.ToString());
		}
		QueueFree();
	}

	public override int getCooldown()
	{
		return cooldown;
	}
}
