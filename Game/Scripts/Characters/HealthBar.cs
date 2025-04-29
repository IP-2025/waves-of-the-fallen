using Godot;
using System;

public partial class HealthBar : ProgressBar
{
	private Health healthNode;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		healthNode = GetParent<Health>();
		MaxValue = healthNode.max_health;
		MinValue = 0;
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var health = GetParent() as Health;
		float current = health.CurHealth;
		Value = current;
	}
}
