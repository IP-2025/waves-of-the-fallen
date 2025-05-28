using Godot;
using System;

public partial class DoubleBlade : Area2D
{
	private DoubleBladeL leftBlade;
	private DoubleBladeR rightBlade;
	private bool useRightNext = true;

	public override void _Ready()
	{
		GD.Print("Start SwordAnimation");
		leftBlade = GetNode<DoubleBladeL>("DoubleBladeL");
		rightBlade = GetNode<DoubleBladeR>("DoubleBladeR");
	}

	private async void OnTimeTimeout()
	{
		if (useRightNext)
		{
			rightBlade?.StartAttack();
		}
		else
		{
			leftBlade?.StartAttack();
		}
		useRightNext = !useRightNext;
		await ToSignal(GetTree().CreateTimer(0.2), "timeout");
	}
}
