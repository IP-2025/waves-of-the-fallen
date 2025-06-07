using Godot;

public partial class Knight : DefaultPlayer
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

		MaxHealth = CharacterManager.LoadHealthByID("3");
		Speed = CharacterManager.LoadSpeedByID("3");

		var healthNode = GetNode<Health>("Health");
		healthNode.max_health = MaxHealth;
		healthNode.ResetHealth();

		GD.Print($"Knight initialized. Speed: {Speed}, MaxHealth: {MaxHealth}");
	}

	public void _on_knight_animation_frame_changed()
	{
		if(GetNode<AnimatedSprite2D>("KnightAnimation").Animation.Equals("walk"))
		if (GetNode<AnimatedSprite2D>("KnightAnimation").Frame%2 == 1)
		{
			SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("playerWalk"), GlobalPosition, -10);
		}
	}

	public override void UseAbility()
	{
		//Ability 1: SelfHeal

		//Ability 2: Shield
		//AddChild(GD.Load<PackedScene>("res://UI/Ability/Ablities/shield.tscn").Instantiate<CharacterBody2D>());
	}
}
