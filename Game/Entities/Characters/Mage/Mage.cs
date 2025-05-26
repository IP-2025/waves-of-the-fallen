using Godot;

public partial class Mage : DefaultPlayer
{
	public override void _Ready()
	{
		HttpRequest = GetNodeOrNull<HttpRequest>("HttpRequest");
		if (HttpRequest == null)
		{
			GD.PrintErr("HttpRequest node not found!");
			return;
		}
		base._Ready();
		
		MaxHealth = CharacterManager.LoadHealthByID("4");
		Speed = CharacterManager.LoadSpeedByID("4");

		var healthNode = GetNode<Health>("Health");
		healthNode.max_health = MaxHealth;
		healthNode.ResetHealth();

		GD.Print($"Mage initialized. Speed: {Speed}, MaxHealth: {MaxHealth}");
	}

	protected override void UseAbility()
	{
		//TODO: Implement Mage's ability
	}
}
