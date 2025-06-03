using Godot;
using System;
using System.Threading.Tasks;
using Game.Utilities.Multiplayer;

public partial class OnlineLocalMenu : Control
{
	private void HeadlessServerInitializedEventHandler()
	{
		throw new NotImplementedException();
	}

	private void _on_button_back_onlineLocal_pressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Menu/Main/mainMenu.tscn");
		SoundManager.Instance.PlayUI();
		GetTree().ChangeSceneToPacked(scene);
	}
	private void _on_button_local_pressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Menu/localMenu.tscn");
		SoundManager.Instance.PlayUI();
		GetTree().ChangeSceneToPacked(scene);
	}
	private void _on_button_online_pressed()
	{

	}

	private void _on_button_solo_pressed()
	{
		Button soloButton = GetNode<Button>("MarginContainer2/VBoxContainer/MarginContainer/HBoxContainer2/Button_Solo");
		soloButton.Disabled = true;
		SoundManager.Instance.PlayUI();

		var characterManager = GetNode<CharacterManager>("/root/CharacterManager");
		int selectedCharacterId = characterManager.LoadLastSelectedCharacterID();

		GameRoot gameScene = ResourceLoader.Load<PackedScene>("res://Utilities/GameRoot/GameRoot.tscn").Instantiate<GameRoot>();
		gameScene._soloSelectedCharacterId = selectedCharacterId;
		gameScene._soloMode = true;
		GetTree().Root.AddChild(gameScene);
		GetTree().CurrentScene = gameScene;
	}
}
