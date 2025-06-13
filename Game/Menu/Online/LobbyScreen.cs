using Godot;
using System;
using Game.Utilities.Backend;
using Game.Utilities.Multiplayer;

public partial class LobbyScreen : Node
{
	
	private Label lobbyCodeLabel;
	private Button startGameButton;
	private Button readyButton;
	public override void _Ready()
	{
	   var lobbyCode = GameState.LobbyCode;
	   lobbyCodeLabel =  GetNode<Label>("MarginContainer2/VBoxContainer/HBoxContainer/lobbyLabel");
	   startGameButton = GetNode<Button>("MarginContainer2/VBoxContainer/MarginContainer/HBoxContainer/_startGameButton");
	   readyButton = GetNode<Button>("MarginContainer2/VBoxContainer/MarginContainer/HBoxContainer/_readyButton");
	GD.Print(lobbyCode);
	   lobbyCodeLabel.Text = "Label - " +lobbyCode;
	}

	public void _on_start_game_pressed()
	{
		
		NetworkManager.Instance.Rpc("NotifyGameStart");
	}
	
	public void _on__ready_button_pressed()
	{
		var characterManager = GetNode<CharacterManager>("/root/CharacterManager");
		int selectedCharacterId = characterManager.LoadLastSelectedCharacterID();
		NetworkManager.Instance.RpcId(1, "SelectCharacter", selectedCharacterId);
	}
	
	
	
}
