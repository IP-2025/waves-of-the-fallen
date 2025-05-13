using Godot;
using System;

public partial class HealthBar : ProgressBar
{
	private Health healthNode;
	private Label healthLabel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		healthLabel = GetNode<Label>("HealthLabel");
		healthNode = GetParent<Health>();
		MaxValue = healthNode.max_health;
		MinValue = 0;
		healthLabel.Text = $"{MaxValue}/{MaxValue}";
		GD.Print($"Max Health applied: {MaxValue}");
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		UpdateHealthBar();
	}

	public void UpdateHealthBar()
	{
		MaxValue = healthNode.max_health;
		Value = healthNode.CurHealth;
		healthLabel.Text = $"{Value}/{MaxValue}";
	}
}
