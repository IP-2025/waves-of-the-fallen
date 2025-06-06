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

	public void _on_mage_animation_frame_changed()
	{
		if(GetNode<AnimatedSprite2D>("MageAnimation").Animation.Equals("walk"))
		if (GetNode<AnimatedSprite2D>("MageAnimation").Frame%2 == 1)
		{
			SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("playerWalk"), GlobalPosition, -10);
		}
	}

	protected override void UseAbility()
	{
		//TODO: Implement Mage's ability increase to int
	}
}
