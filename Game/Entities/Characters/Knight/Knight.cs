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

		if (MaxHealth <= 0 && Speed <= 0)
		{
			MaxHealth = CharacterManager.LoadHealthByID("3");
			Speed = CharacterManager.LoadSpeedByID("3");
			Dexterity = CharacterManager.LoadDexterityByID("3");
			Strength = CharacterManager.LoadStrengthByID("3");
			Intelligence = CharacterManager.LoadIntelligenceByID("3");
		}

		GD.Print($"Knight initialized. Speed: {Speed}, MaxHealth: {MaxHealth}, Dex: {Dexterity}, Str: {Strength}, Int: {Intelligence}");
	}

	public void _on_knight_animation_frame_changed()
	{
		if(GetNode<AnimatedSprite2D>("KnightAnimation").Animation.Equals("walk"))
		if (GetNode<AnimatedSprite2D>("KnightAnimation").Frame%2 == 1)
		{
			SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("playerWalk"), GlobalPosition, -10);
		}
	}

	protected override void UseAbility()
	{
		//TODO: Implement Knight's ability
	}
}
