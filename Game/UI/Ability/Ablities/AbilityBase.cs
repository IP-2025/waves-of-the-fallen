using Godot;
using System;
using System.Diagnostics;

public partial class AbilityBase : Node2D
{
	private Node2D _parent;
	private int cooldown = 99;

	public override void _Ready()
	{
		_parent = GetParent<Node2D>();
	}

	public virtual int getCooldown()
	{
		return cooldown;
	}
}
