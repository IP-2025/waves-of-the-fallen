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

		MaxHealth = CharacterManager.LoadHealthByID("2");
		Speed = CharacterManager.LoadSpeedByID("2");

		var healthNode = GetNode<Health>("Health");
		healthNode.max_health = MaxHealth;
		healthNode.ResetHealth();

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

	public override void UseAbility()
	{
		//Ability 1: SpeedUp
		AddChild(GD.Load<PackedScene>("res://UI/Ability/Ablities/speed_up.tscn").Instantiate<Node2D>());
		SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("dash"), GlobalPosition, -10);
		//Ability 2: WeaponEnhance
	}
}
