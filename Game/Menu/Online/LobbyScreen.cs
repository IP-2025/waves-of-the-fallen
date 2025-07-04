using Godot;
using System;
using Game.Utilities.Backend;
using Game.Utilities.Multiplayer;

public partial class LobbyScreen : Node
{
	
	private Label lobbyCodeLabel;
	private Button startGameButton;
	private Button readyButton;
	private Button backButton;

	public override void _Ready()
	{
	   var lobbyCode = GameState.LobbyCode;
	   lobbyCodeLabel =  GetNode<Label>("MarginContainer2/VBoxContainer/HBoxContainer/lobbyLabel");
	   startGameButton = GetNode<Button>("MarginContainer2/VBoxContainer/MarginContainer/HBoxContainer/_startGameButton");
	   readyButton = GetNode<Button>("MarginContainer2/VBoxContainer/MarginContainer/HBoxContainer/_readyButton");
	   backButton = GetNode<Button>("MarginContainer2/VBoxContainer/HBoxContainer/Button_Back");

	   lobbyCodeLabel.Text = "Label - " +lobbyCode;
	}

	public void _on_start_game_pressed()
	{
		
		NetworkManager.Instance.Rpc("NotifyGameStart");
		GD.Print("Start Game Pressed");
	}
	
	public void _on__ready_button_pressed()
	{
		var characterManager = GetNode<CharacterManager>("/root/CharacterManager");
		int selectedCharacterId = characterManager.LoadLastSelectedCharacterID();
		var health = characterManager.LoadHealthByID(selectedCharacterId.ToString());
		var speed = characterManager.LoadSpeedByID(selectedCharacterId.ToString());
		NetworkManager.Instance.RpcId(1, "SelectCharacter", selectedCharacterId, health, speed);
		GD.Print("Ready Button Pressed");
	}
	
	private void _on_button_back_pressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Menu/Main/mainMenu.tscn");
		SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
		GetTree().ChangeSceneToPacked(scene);
	}
	
}
