using Godot;
using System;

public partial class HealthBar : ProgressBar
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MaxValue = 100;
		MinValue = 0;
		Value = 100;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var healthNode = GetParent() as Health;
		float current = healthNode.CurHealth;
		Value = current;
	}
}
