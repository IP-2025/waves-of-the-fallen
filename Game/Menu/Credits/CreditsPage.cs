using Godot;
using System;

public partial class CreditsPage : Control
{

	private void _on_button_BackToMainMenu_pressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Menu/Main/mainMenu.tscn");
		SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
		GetTree().ChangeSceneToPacked(scene);
	}
}
