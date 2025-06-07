using Godot;
using System;

public partial class SpeedUp : Node2D
{
	private Node2D _parent;
	private float tempSpeed;

	public override void _Ready()
	{
		_parent = GetParent<Node2D>();
		if (_parent is DefaultPlayer player)
		{
			tempSpeed = player.Speed;
			player.Speed *= 3;
		}
	}

	private void _on_speed_timer_timeout()
	{
		if (_parent is DefaultPlayer player)
		{
			player.Speed = tempSpeed;
		}
		QueueFree();
	}
}
