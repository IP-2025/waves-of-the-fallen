using Godot;

public partial class Archer : DefaultPlayer
{
	public Archer()
	{
		MaxHealth = 175;
		Speed = 400.0f;
	}

	public override void _Ready()
	{
		HttpRequest = GetNodeOrNull<HttpRequest>("HttpRequest");
		if (HttpRequest == null)
		{
			GD.PrintErr("HttpRequest node not found!");
			return;
		}
		base._Ready();

		var healthNode = GetNode<Health>("Health");
		healthNode.max_health = MaxHealth; 
		healthNode.ResetHealth();

		GD.Print($"Archer initialized. Speed: {Speed}, MaxHealth: {MaxHealth}");
	}

	protected override void UseAbility()
	{
		//TODO: Implement Archer's ability
	}
}
