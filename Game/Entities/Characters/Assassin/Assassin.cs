using System.Diagnostics;
using Godot;

public partial class Assassin : DefaultPlayer
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

		if ( MaxHealth <= 0 && Speed <= 0)
		{
			MaxHealth = CharacterManager.LoadHealthByID("2");
			Speed = CharacterManager.LoadSpeedByID("2");
		}

		GD.Print($"Assassin initialized. Speed: {Speed}, MaxHealth: {MaxHealth}");
	}

	public void _on_assassin_animation_frame_changed()
	{
		if(GetNode<AnimatedSprite2D>("AssassinAnimation").Animation.Equals("walk"))
		if (GetNode<AnimatedSprite2D>("AssassinAnimation").Frame%2 == 1)
		{
			SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("playerWalk"), GlobalPosition, -10);
		}
	}

	protected override void UseAbility()
	{
		//TODO: Implement Assassin's ability
	}
}
