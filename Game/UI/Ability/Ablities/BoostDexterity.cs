using Godot;
using System;

public partial class BoostDexterity : Node2D
{
	private Node2D _parent;
	private int tempDex;
	private int boostAmount = 2;

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
}
