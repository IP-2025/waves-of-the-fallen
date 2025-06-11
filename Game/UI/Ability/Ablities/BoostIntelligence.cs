using Godot;
using System;

public partial class BoostIntelligence : AbilityBase
{
	private Node2D _parent;
	private int tempInt;
	private int boostAmount = 2;
	private int cooldown = 15;

	public override void _Ready()
	{
		_parent = GetParent<Node2D>();
		if (_parent is DefaultPlayer player)
		{
			tempInt = player.Intelligence;
			player.Intelligence *= boostAmount;
		}
	}

	private void _on_boost_int_timer_timeout()
	{
		if (_parent is DefaultPlayer player)
		{
			player.Intelligence = tempInt;
		}
		QueueFree();
	}

	public override int getCooldown()
	{
		return cooldown;
	}
}
