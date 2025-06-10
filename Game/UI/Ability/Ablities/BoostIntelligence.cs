using Godot;
using System;

public partial class BoostIntelligence : Node2D
{
	private Node2D _parent;
	private int tempInt;
	private int boostAmount = 2;

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
}
