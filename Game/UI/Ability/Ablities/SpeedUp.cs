using Godot;
using System;

public partial class SpeedUp : AbilityBase
{
	private Node2D _parent;
	private float tempSpeed;
	private int cooldown = 5;

	public override void _Ready()
	{
		_parent = GetParent<Node2D>();
		if (_parent is DefaultPlayer player)
		{
			tempSpeed = player.Speed;
			player.Speed *= 3;
			SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("dash"), GlobalPosition, -10);
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

	public override int getCooldown()
	{
		return cooldown;
	}
}
