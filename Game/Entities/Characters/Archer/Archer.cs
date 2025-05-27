using Godot;

public partial class Archer : DefaultPlayer
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

		MaxHealth = CharacterManager.LoadHealthByID("1");
		Speed = CharacterManager.LoadSpeedByID("1");

		var healthNode = GetNode<Health>("Health");
		healthNode.max_health = MaxHealth;
		healthNode.ResetHealth();

		GD.Print($"Archer initialized. Speed: {Speed}, MaxHealth: {MaxHealth}");
	}

	public void _on_archer_animation_frame_changed()
	{
		if(GetNode<AnimatedSprite2D>("ArcherAnimation").Animation.Equals("walk"))
		if (GetNode<AnimatedSprite2D>("ArcherAnimation").Frame%2 == 1)
		{
			SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("playerWalk"), GlobalPosition, -10);
		}
	}
	protected override void UseAbility()
	{
		//TODO: Implement Archer's ability
	}
}
