using Godot;
using System;

public partial class Shield : CharacterBody2D
{
	private Node2D _parent;
	private Node2D health;
	private int cooldown = 15;
	public override void _Ready()
	{
		_parent = GetParent<Node2D>();
		health = _parent.GetNode<Health>("Health");
	}

	public void _on_shield_timer_timeout()
	{
		QueueFree();
	}

	public int getCooldown()
	{
		return cooldown;
	}
}
