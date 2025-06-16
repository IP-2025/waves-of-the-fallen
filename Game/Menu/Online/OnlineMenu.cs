using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using Game.Utilities.Backend;
using Game.Utilities.Multiplayer;

public partial class OnlineMenu : Node
{
	private Button joinLobbyButton;
	private Button hostLobbyButton;
	private Button backButton;
	private LineEdit lobbyCodeInput;
	private HttpRequest httpRequest;

	public override void _Ready()
	{
		joinLobbyButton = GetNode<Button>("MarginContainer2/VBoxContainer/MarginContainer/HBoxContainer/JoinLobbyButton");
		hostLobbyButton = GetNode<Button>("MarginContainer2/VBoxContainer/MarginContainer/HBoxContainer/HostLobbyButton");
		backButton = GetNode<Button>("MarginContainer2/VBoxContainer/HBoxContainer/Button_Back");
		lobbyCodeInput = GetNode<LineEdit>("LobbyCodeInput");
		httpRequest = GetNode<HttpRequest>("HTTPRequest");

		joinLobbyButton.Pressed += OnJoinLobbyPressed;
		hostLobbyButton.Pressed += OnHostLobbyPressed;
		httpRequest.RequestCompleted += OnRequestCompleted;
	}

	private void _on_button_back_pressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Menu/Main/mainMenu.tscn");
		SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
		GetTree().ChangeSceneToPacked(scene);
	}

	private void OnJoinLobbyPressed()
	{
		string code = lobbyCodeInput.Text.Trim();
		if (string.IsNullOrEmpty(code))
		{
			GD.Print("Lobby code is empty. Insert shame here.");
			return;
		}

		string url = $"{ServerConfig.BaseUrl}/api/v1/game/join";
		var body = new Godot.Collections.Dictionary{
			{ "lobbyCode", code }
		};
		SendPostRequest(url, body);
	}

	private void OnHostLobbyPressed()
	{
		string url = $"{ServerConfig.BaseUrl}/api/v1/game/start";
		var body =new Godot.Collections.Dictionary {
			
		};
		SendPostRequest(url, body);
	}

	private void SendPostRequest(string url, Godot.Collections.Dictionary body)
	{
		string json = Json.Stringify(body);
		var headers = new string[] { "Content-Type: application/json" };
		var err = httpRequest.Request(url, headers, HttpClient.Method.Post, json);
		if (err != Error.Ok)
		{
			GD.PrintErr("HTTPRequest failed to send. Time to cry. Error: ", err);
		}
	}

	private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		string responseText = Encoding.UTF8.GetString(body);
		GD.Print("Response Code: " + responseCode);
		GD.Print("Body: " + responseText);
	
		// Try parsing it as JSON (assuming the response is JSON)
		var json = Json.ParseString(responseText).AsGodotDictionary();
		GD.Print("JSON: " + json);
		// if (json["status"].AsString() != "201")
		// {
		// 	GD.PrintErr("JSON parsing failed because the universe is cruel. Error: ");
		// 	return;
		// }
		GD.Print(json);
		try
		{
			int udpPort = int.Parse(json["udpPort"].AsString().Replace(".0", ""));
			int rpcPort = int.Parse(json["rpcPort"].AsString().Replace(".0", ""));
			string lobbyCode = json["lobbyCode"].AsString();
			NetworkManager.Instance.InitClient("http://localhost", udpPort, rpcPort);
			GameState.LobbyCode = lobbyCode;
			var characterManager = GetNode<CharacterManager>("/root/CharacterManager");
			int selectedCharacterId = characterManager.LoadLastSelectedCharacterID();
			var erra = NetworkManager.Instance.RpcId(1, "SelectCharacter", selectedCharacterId);
			GD.Print("Result " + erra);
			
			
			
			
			var timer2 = new Timer();
			AddChild(timer2);
			timer2.WaitTime = 10;
			timer2.OneShot = true;
			timer2.Timeout += () =>
			{
				GD.Print("Sending character from peer: "+ Multiplayer.GetUniqueId());
				var characterManager = GetNode<CharacterManager>("/root/CharacterManager");
				int selectedCharacterId = characterManager.LoadLastSelectedCharacterID();
				var erra = NetworkManager.Instance.RpcId(1, "SelectCharacter", selectedCharacterId);
				GD.Print("Result " + erra);
			};
			timer2.Start();
			
			
			// NetworkManager.Instance.RpcId(1, "SelectCharacter", selectedCharacterId);
			
			var scene = ResourceLoader.Load<PackedScene>("res://Menu/Online/LobbyScreen.tscn");
			GetTree().ChangeSceneToPacked(scene);
		}
		catch (Exception e)
		{
			GD.PrintErr(e.Message);
		}

		


		//
		// var data = (Godot.Collections.Dictionary)json;
		// GD.Print("Parsed Body Data: " + data.ToString());
		//
		// // Access specific values like this:
		// if (data.Contains("message"))
		// {
		// 	string message = data["message"].ToString();
		// 	GD.Print("Server says: " + message);
		// }
		//

	}

}
