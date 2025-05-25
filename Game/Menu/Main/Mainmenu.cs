using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Utilities.Backend;
using Godot;
using Godot.Collections;

namespace Game.Menu.Main;

internal class UnlockedCharacter
{
	public int CharacterId { get; init; }
	public int Level { get; init; }
}
public partial class Mainmenu : Control
{
	private CharacterManager _characterManager;
	
	private HttpRequest _unlockRequest;
	private HttpRequest _progressCheckRequest;
	private Button _localButton;
	private Button _onlineButton;
	private ColorRect _colorRect;
	private HttpRequest _syncLocalOnServerRequest;

	private List<UnlockedCharacter> _localProgress;
	private List<UnlockedCharacter> _onlineProgress;

	public override void _Ready()
	{
		_localProgress = [];
		_onlineProgress = [];
		_localProgress.Clear();
		_onlineProgress.Clear();
	  
		_characterManager = GetNode<CharacterManager>("/root/CharacterManager");
	  
		_unlockRequest = GetNode<HttpRequest>("%HTTPRequest");
		_progressCheckRequest = GetNode<HttpRequest>("%ProgressCheckRequest");
		_localButton = GetNode<Button>("%LocalButton");
		_onlineButton = GetNode<Button>("%OnlineButton");
		_colorRect = GetNode<ColorRect>("%GreyedOut");
		_syncLocalOnServerRequest = GetNode<HttpRequest>("%SyncLocalOnServerRequest");
		
		_unlockRequest.Connect("request_completed", new Callable(this, nameof(OnRequestCompleted)));
		_progressCheckRequest.Connect("request_completed", new Callable(this, nameof(OnProgressCheckRequestCompleted)));
		_localButton.Connect("pressed", new Callable(this, nameof(OnLocalButtonPressed)));
		_onlineButton.Connect("pressed", new Callable(this, nameof(OnOnlineButtonPressed)));
		_syncLocalOnServerRequest.Connect("request_completed", new Callable(this, nameof(OnSyncLocalOnServerRequest)));
	  
		// Check for mismatch between server and local character data if online
		if (GameState.CurrentState != ConnectionState.Online) return;
		var headers = new[]
		{
			"Content-Type: application/json",
			"Authorization: Bearer " + SecureStorage.LoadToken()
		};
		var err = _progressCheckRequest.Request(
			$"{ServerConfig.BaseUrl}/api/v1/protected/getAllUnlockedCharacters",
			headers,
			HttpClient.Method.Post
		);

		if (err != Error.Ok)
			GD.PrintErr($"AuthRequest error: {err}");
	}
	private void OnSyncLocalOnServerRequest(long result, long responseCode, string[] headers, byte[] body)
	{
		var receivedString = Encoding.UTF8.GetString(body);
		GD.Print("Antwort empfangen: " + receivedString);

		GD.Print(responseCode == 200
			? "Local progress synchronized with server."
			: "Error synchronizing local progress with server.");
	}

	private void OnLocalButtonPressed()
	{
		var localProgressArray = new Godot.Collections.Array();

		foreach (var characterData in _localProgress.Select(character => new Dictionary
		         {
			         { "character_id", character.CharacterId },
			         { "level", character.Level }
		         }))
		{
			localProgressArray.Add(characterData);
		}

		var body = Json.Stringify(new Dictionary
		{
			{ "local_progress", localProgressArray }
		});


		var headers = new[]
		{
			"Content-Type: application/json",
			"Authorization: Bearer " + SecureStorage.LoadToken()
		};
		var err = _unlockRequest.Request(
			$"{ServerConfig.BaseUrl}/api/v1/protected/progressSync",
			headers,
			HttpClient.Method.Post,
			body
		);

		if (err != Error.Ok)
			GD.PrintErr($"AuthRequest error: {err}");

		_colorRect.Visible = false;
		_characterManager.SaveLastSelectedCharacterID(1);
	}

	private void OnOnlineButtonPressed()
	{
		GD.Print("Synchronizing local state with server…");

		const int maxCharacterId = 4;
		for (var id = 1; id <= maxCharacterId; id++)
		{
			var onlineEntry = _onlineProgress
				.FirstOrDefault(c => c.CharacterId == id);

			if (onlineEntry != null)
			{
				_characterManager.UpdateCharacter(
					id.ToString(),
					onlineEntry.Level,
					1
				);
			}
			else
			{
				_characterManager.UpdateCharacter(
					id.ToString(),
					1,
					0
				);
			}
		}

		_colorRect.Visible = false;
		_characterManager.SaveLastSelectedCharacterID(1);
		GD.Print("Local progress is now in sync with online.");
	}
	
	private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		var receivedString = Encoding.UTF8.GetString(body);
		GD.Print("Antwort empfangen: " + receivedString);

		GD.Print(responseCode == 200 ? "Character unlocked" : "Error unlocking character");
	}

	private void OnProgressCheckRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		var receivedString = Encoding.UTF8.GetString(body);

		if (responseCode == 200)
		{
			try
			{
				var json = new Json();
				if (json.Parse(receivedString) != Error.Ok)
				{
					GD.PrintErr("Failed to parse JSON!");
					return;
				}

				// 2) Pull the root dictionary out of the Variant:
				var root = (Dictionary)json.Data;

				var unlockedVariant = root["unlocked_characters"];
				var unlockedArray = (Godot.Collections.Array)unlockedVariant;

				foreach (var element in unlockedArray)
				{
					var character = (Dictionary)element;
					var characterId = (int)character["character_id"];
					var level = (int)character["level"];

					var unlockedCharacter = new UnlockedCharacter
					{
						CharacterId = characterId,
						Level = level
					};
					_onlineProgress.Add(unlockedCharacter);
				}

				_localProgress = LoadLocalProgress();
				if (EqualProgress(_localProgress, _onlineProgress)) return;
				GD.Print("Mismatch between local and online progress detected.");
				_colorRect.Visible = true;
			}
			catch (Exception ex)
			{
				GD.PrintErr("Fehler beim Verarbeiten der Antwort: " + ex.Message);
			}
		}
		else
		{
			GD.Print("Fehler beim Abrufen der Charakterdaten: " + responseCode);
		}
	}
	
	private List<UnlockedCharacter> LoadLocalProgress()
	{
		var unlockedCharacters = new List<UnlockedCharacter>();

		for (var i = 1; i <= 4; i++) // Angenommen, es gibt 4 Charaktere
		{
			var characterId = i.ToString();
			if (!_characterManager.LoadIsUnlocked(characterId)) continue;
			var character = new UnlockedCharacter
			{
				CharacterId = i,
				Level = _characterManager.LoadLevelByID(characterId)
			};

			unlockedCharacters.Add(character);
		}

		return unlockedCharacters;
	}
	
	private bool EqualProgress(List<UnlockedCharacter> localProgress, List<UnlockedCharacter> onlineProgress)
	{
		localProgress.Sort((a, b) => a.CharacterId.CompareTo(b.CharacterId));
		onlineProgress.Sort((a, b) => a.CharacterId.CompareTo(b.CharacterId));

		// print both lists for debugging
		GD.Print("Local Progress: " + string.Join(", ", localProgress));
		GD.Print("Online Progress: " + string.Join(", ", onlineProgress));

		if (localProgress.Count != onlineProgress.Count)
		{
			return false;
		}

		for (var i = 0; i < localProgress.Count; i++)
		{
			var local = localProgress[i];
			var online = onlineProgress[i];

			if (local.CharacterId != online.CharacterId || local.Level != online.Level)
			{
				return false;
			}
		}

		return true;
	}
	
	private void _on_button_charactermenu_pressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Menu/Character/characterMenu.tscn");
		GetTree().ChangeSceneToPacked(scene);
	}
	
	private void _on_button_settings_pressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Menu/Settings/settingsMenu.tscn");
		SoundManager.Instance.PlayUI();
		GetTree().ChangeSceneToPacked(scene);
	}
	
	private void _on_button_play_pressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Menu/online_localMenu.tscn");
		SoundManager.Instance.PlayUI();
		GetTree().ChangeSceneToPacked(scene);
	}

	private void _on_button_highscore_pressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Menu/HighscoreList/highscore_screen.tscn");
		SoundManager.Instance.PlayUI();
		GetTree().ChangeSceneToPacked(scene);
	}
}