using Godot;
using System;

public partial class BoostDexterity : AbilityBase
{
	private Node2D _parent;
	private int tempDex;
	private int boostAmount = 2;
	private int cooldown = 15;

	public override void _Ready()
	{
		_parent = GetParent<Node2D>();
		if (_parent is DefaultPlayer player)
		{
			tempDex = player.Dexterity;
			player.Dexterity *= boostAmount;
		}
	}

	private void _on_boost_dex_timer_timeout()
	{
		if (_parent is DefaultPlayer player)
		{
			player.Dexterity = tempDex;
		}
		QueueFree();
	}

	public override int getCooldown()
	{
		return cooldown;
	}
}
