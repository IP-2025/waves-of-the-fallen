using System;
using Godot;
using Game.Utilities.Backend;

namespace Game.Menu.HighscoreList;

public partial class HighscoreScreen : Control
{
	private HttpRequest _personalScoreRequest;
	private HttpRequest _topPlayerRequest;
	private Button _backButton;
	private Button _mainMenuButton;

	public override void _Ready()
	{
		_personalScoreRequest = GetNode<HttpRequest>("Panel/PersonalScoreRequest");
		_topPlayerRequest = GetNode<HttpRequest>("Panel/TopPlayersRequest");
		_backButton = GetNode<Button>("Panel/BackButton");
		_mainMenuButton = GetNode<Button>("Panel/Offline/Button");


		_personalScoreRequest.Connect("request_completed", new Callable(this, nameof(OnPersonalScoreRequestCompleted)));
		_topPlayerRequest.Connect("request_completed", new Callable(this, nameof(OnTopPlayersRequestCompleted)));
		_backButton.Connect("pressed", new Callable(this, nameof(OnBackButtonPressed)));
		_mainMenuButton.Connect("pressed", new Callable(this, nameof(OnMainMenuButtonPressed)));


		if (GameState.CurrentState == ConnectionState.Online)
		{
			var headers = new[]
			{
				"Content-Type: application/json",
				"Authorization: Bearer " + SecureStorage.LoadToken()
			};
			var err = _topPlayerRequest.Request(
				$"{Server.BaseUrl}/api/v1/protected/highscore/top",
				headers
			);

			if (err != Error.Ok)
				GD.PrintErr($"AuthRequest error: {err}");


			var headers2 = new[]
			{
				"Content-Type: application/json",
				"Authorization: Bearer " + SecureStorage.LoadToken()
			};
			var err2 = _personalScoreRequest.Request(
				$"{Server.BaseUrl}/api/v1/protected/highscore/getUserHighscore",
				headers2,
				HttpClient.Method.Post
			);

			if (err2 != Error.Ok)
				GD.PrintErr($"AuthRequest error: {err2}");
		}
		else
		{
			var offlinePanel = GetNode<Panel>("Panel/Offline");
			offlinePanel.Visible = true;
		}
	}

	private void OnPersonalScoreRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		if (responseCode == 200)
		{
			var bodyText = System.Text.Encoding.UTF8.GetString(body);
			var json = new Json();
			var parseErr = json.Parse(bodyText);
			if (parseErr == Error.Ok)
			{
				var data = (Godot.Collections.Dictionary)json.GetData();
				var highScore = (Godot.Collections.Dictionary)data["highScore"];

				var playerScore = GetNode<ColorRect>("Panel/PlayerScore");
				playerScore.GetNode<Label>("Position").Text = "-";
				playerScore.GetNode<Label>("Name").Text = "Me";
				playerScore.GetNode<Label>("Score").Text = highScore["highScore"].ToString();

				var rawTimestamp = highScore["timeStamp"].ToString();
				var formattedTime = rawTimestamp;
				if (DateTime.TryParse(rawTimestamp, out var dt))
					formattedTime = dt.ToLocalTime().ToString("dd.MM.yyyy HH:mm");

				playerScore.GetNode<Label>("Time").Text = formattedTime;
			}
			else
			{
				GD.PrintErr("Error parsing JSON response.");
			}
		}
		else
		{
			GD.PrintErr($"Error fetching personal score: {responseCode}");
		}
	}

	private void OnTopPlayersRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		if (responseCode == 200)
		{
			var bodyText = System.Text.Encoding.UTF8.GetString(body);
			var json = new Json();
			var parseErr = json.Parse(bodyText);
			if (parseErr == Error.Ok)
			{
				var data = (Godot.Collections.Dictionary)json.GetData();
				var highScoreList = (Godot.Collections.Array)data["highScoreList"];
				var entryScene = GD.Load<PackedScene>("res://Menu/HighscoreList/entry.tscn");
				var vbox = GetNode<VBoxContainer>("Panel/List/VBoxContainer");
				vbox.ClearChildren();

				for (var i = 0; i < highScoreList.Count; i++)
				{
					var scoreDict = (Godot.Collections.Dictionary)highScoreList[i];
					var player = (Godot.Collections.Dictionary)scoreDict["player"];

					var entry = entryScene.Instantiate<Control>();
					entry.GetNode<Label>("Position").Text = (i + 1).ToString();
					entry.GetNode<Label>("Name").Text = player["username"].ToString();
					entry.GetNode<Label>("Score").Text = scoreDict["highScore"].ToString();

					var rawTimestamp = scoreDict["timeStamp"].ToString();
					var formattedTime = rawTimestamp;
					if (DateTime.TryParse(rawTimestamp, out var dt))
						formattedTime = dt.ToLocalTime().ToString("dd.MM.yyyy HH:mm");

					entry.GetNode<Label>("Time").Text = formattedTime;
					vbox.AddChild(entry);
				}
			}
			else
			{
				GD.PrintErr("Error parsing JSON response.");
			}
		}
		else
		{
			GD.PrintErr($"Error fetching top players: {responseCode}");
		}
	}

	private void OnBackButtonPressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Menu/Main/mainMenu.tscn");
		if (scene == null) GD.PrintErr("Main Menu Scene not found");
		SoundManager.Instance.PlayUI();
		GetTree().ChangeSceneToPacked(scene);
	}
	
	private void OnMainMenuButtonPressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Menu/Main/mainMenu.tscn");
		if (scene == null) GD.PrintErr("Main Menu Scene not found");
		SoundManager.Instance.PlayUI();
		GetTree().ChangeSceneToPacked(scene);
	}
}

public static class NodeExtensions
{
	public static void ClearChildren(this Node node)
	{
		foreach (var child in node.GetChildren())
			child.QueueFree();
	}
}
