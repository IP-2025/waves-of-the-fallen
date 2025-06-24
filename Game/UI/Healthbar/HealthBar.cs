using Godot;

public partial class HealthBar : TextureProgressBar
{
	private Health healthNode;
	private Label healthLabel;
	public override void _Ready()
	{
		healthLabel = GetNode<Label>("HealthLabel");
		healthNode = GetParent<Health>();
		MaxValue = healthNode.max_health;
		MinValue = 0;
		healthLabel.Text = $"{Value}";
	}

	public override void _Process(double delta)
	{
		UpdateHealthBar();
	}

	public void UpdateHealthBar()
	{
		MaxValue = healthNode.max_health;
		Value = healthNode.CurHealth;
		healthLabel.Text = $"{Value}";
	}
}
