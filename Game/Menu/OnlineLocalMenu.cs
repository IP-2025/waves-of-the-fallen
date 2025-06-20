using Godot;
using System;
using System.Threading.Tasks;
using Game.Utilities.Multiplayer;

public partial class OnlineLocalMenu : Control
{
	private void _on_button_back_onlineLocal_pressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Menu/Main/mainMenu.tscn");
		SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
		GetTree().ChangeSceneToPacked(scene);
	}
	private void _on_button_local_pressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Menu/localMenu.tscn");
		SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
		GetTree().ChangeSceneToPacked(scene);
	}
	private void _on_button_online_pressed()
	{

		var scene = ResourceLoader.Load<PackedScene>("res://Menu/Online/OnlineMenu.tscn");
		SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
		GetTree().ChangeSceneToPacked(scene);
	}

	private void _on_button_solo_pressed()
	{
		NetworkManager.Instance.SoloMode = true;

		Button soloButton = GetNode<Button>("MarginContainer2/VBoxContainer/MarginContainer/HBoxContainer2/Button_Solo");
		soloButton.Disabled = true;
		SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
		var gameScene = ResourceLoader.Load<PackedScene>("res://Utilities/GameRoot/GameRoot.tscn");
		GetTree().ChangeSceneToPacked(gameScene);
	}

	public override void _Notification(int what)
	{
		if (what == NotificationWMGoBackRequest)
		{
			_on_button_back_onlineLocal_pressed();
		}
		
	}
}
