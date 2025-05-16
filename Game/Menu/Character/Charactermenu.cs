using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using System.Text;
using Game.Utilities.Backend;
using Godot.Collections;

namespace Game.Scripts.Menu;

class UnlockedCharacter
{
	public int CharacterId { get; init; }
	public int Level { get; init; }
}

public partial class Charactermenu : Control
{
	private CharacterManager _characterManager;
	private Label _labelCharacterName;
	private Button _currentlySelectedCharacter;
	private Button _oldSelectedCharacter;
	private Button _buttonUpgradeUnlock;
	private Button _buttonSelect;

	private Label _labelHealth;
	private Label _labelSpeed;
	private Label _labelDexterity;
	private Label _labelIntelligence;

	private HttpRequest _unlockRequest;
	private HttpRequest _levelUpRequest;
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

		_labelHealth = GetNode<Label>("%Label_health");
		_labelSpeed = GetNode<Label>("%Label_speed");
		_labelDexterity = GetNode<Label>("%Label_dexterity");
		_labelIntelligence = GetNode<Label>("%Label_intelligence");

		_labelCharacterName = GetNode<Label>("%Label_SelectedCharacterName");
		_buttonUpgradeUnlock = GetNode<Button>("%Button_UpgradeUnlock");
		_buttonSelect = GetNode<Button>("%Button_Select");

		_unlockRequest = GetNode<HttpRequest>("%HTTPRequest");
		_levelUpRequest = GetNode<HttpRequest>("%LevelUpRequest");
		_progressCheckRequest = GetNode<HttpRequest>("%ProgressCheckRequest");

		_localButton = GetNode<Button>("%LocalButton");
		_onlineButton = GetNode<Button>("%OnlineButton");
		_colorRect = GetNode<ColorRect>("%GreyedOut");
		_syncLocalOnServerRequest = GetNode<HttpRequest>("%SyncLocalOnServerRequest");


		_unlockRequest.Connect("request_completed", new Callable(this, nameof(OnRequestCompleted)));
		_levelUpRequest.Connect("request_completed", new Callable(this, nameof(OnLevelUpRequestCompleted)));
		_progressCheckRequest.Connect("request_completed", new Callable(this, nameof(OnProgressCheckRequestCompleted)));

		_localButton.Connect("pressed", new Callable(this, nameof(OnLocalButtonPressed)));
		_onlineButton.Connect("pressed", new Callable(this, nameof(OnOnlineButtonPressed)));
		_syncLocalOnServerRequest.Connect("request_completed", new Callable(this, nameof(OnSyncLocalOnServerRequest)));

		var selectedId = _characterManager.LoadLastSelectedCharacterID();
		_currentlySelectedCharacter = GetNode<Button>($"%Button_Character{selectedId}");
		_on_button_select_pressed();
		SetCharacterPageValuesFromFile($"{selectedId}");


		// Check for mismatch between server and local character data if online
		if (GameState.CurrentState != ConnectionState.Online) return;
		var headers = new[]
		{
			"Content-Type: application/json",
			"Authorization: Bearer " + SecureStorage.LoadToken()
		};
		var err = _progressCheckRequest.Request(
			$"{Server.BaseUrl}/api/v1/protected/getAllUnlockedCharacters",
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

		if (responseCode == 200)
		{
			GD.Print("Local progress synchronized with server.");
		}
		else
		{
			GD.Print("Error synchronizing local progress with server.");
		}
	}

	private void OnLocalButtonPressed()
	{
		var localProgressArray = new Godot.Collections.Array();

		foreach (var character in _localProgress)
		{
			var characterData = new Dictionary
			{
				{ "character_id", character.CharacterId },
				{ "level", character.Level }
			};
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
			$"{Server.BaseUrl}/api/v1/protected/progressSync",
			headers,
			HttpClient.Method.Post,
			body
		);

		if (err != Error.Ok)
			GD.PrintErr($"AuthRequest error: {err}");

		_colorRect.Visible = false;
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
		GD.Print("Local progress is now in sync with online.");
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
				if (!EqualProgress(_localProgress, _onlineProgress))
				{
					// DO something
					GD.Print("Mismatch between local and online progress detected.");
					_colorRect.Visible = true;
				}
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


	private void OnLevelUpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		var receivedString = Encoding.UTF8.GetString(body);
		GD.Print("Antwort empfangen: " + receivedString);

		GD.Print(responseCode == 200 ? "Character unlocked" : "Error unlocking character");
	}

	private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		var receivedString = Encoding.UTF8.GetString(body);
		GD.Print("Antwort empfangen: " + receivedString);

		GD.Print(responseCode == 200 ? "Character unlocked" : "Error unlocking character");
	}


	// set stats and name to values from file (see CharacterManager.cs)
	private void SetCharacterPageValuesFromFile(string characterId)
	{
		_labelCharacterName.Text =
			$"{_characterManager.LoadNameByID(characterId)} - Lvl.{_characterManager.LoadLevelByID(characterId)}";
		_labelHealth.Text = $"Health {_characterManager.LoadHealthByID(characterId)}";
		_labelSpeed.Text = $"Speed {_characterManager.LoadSpeedByID(characterId)}";
		_labelDexterity.Text = $"Dexterity {_characterManager.LoadDexterityByID(characterId)}";
		_labelIntelligence.Text = $"Intelligence {_characterManager.LoadIntelligenceByID(characterId)}";
	}


	private void _on_button_back_charactermenu_pressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Menu/Main/mainMenu.tscn");
		SoundManager.Instance.PlayUI();
		GetTree().ChangeSceneToPacked(scene);
	}

	//wenn ein character button angeklickt wird
	public void _characterSelected(ButtonsCharacterSelection button)
	{
		if (button == null || _labelCharacterName == null) return;
		SetButtonStyle(_currentlySelectedCharacter, Color.Color8(0x4F, 0x4F, 0x4F), false);

		SetCharacterPageValuesFromFile(button.Text);
		_currentlySelectedCharacter = button;

		SetButtonStyle(_currentlySelectedCharacter, Color.Color8(0x2C, 0xC7, 0xFF), false);

		if (!_characterManager.LoadIsUnlocked(button.Text))
		{
			//check if character is unlocken. locked=true
			_buttonUpgradeUnlock.Text = "Unlock";
			_buttonSelect.Hide();
		}
		else
		{
			_buttonUpgradeUnlock.Text = "Upgrade";
			_buttonSelect.Show();
		}
	}

	private void SetButtonStyle(Button button, Color color, bool addBorder)
	{
		var style = button.GetThemeStylebox("normal") as StyleBoxFlat;
		if (style == null) return;
		var newStyle = (StyleBoxFlat)style.Duplicate();
		newStyle.BgColor = color;
		newStyle.BorderColor = new Color(1f, 0f, 0f);

		if (addBorder)
		{
			newStyle.BorderColor = new Color(1f, 0f, 0f);
			newStyle.SetBorderWidth(Side.Top, 3);
			newStyle.SetBorderWidth(Side.Bottom, 3);
			newStyle.SetBorderWidth(Side.Left, 3);
			newStyle.SetBorderWidth(Side.Right, 3);
		}


		button.AddThemeStyleboxOverride("normal", newStyle);
		button.AddThemeStyleboxOverride("hover", newStyle);
		button.AddThemeStyleboxOverride("pressed", newStyle);
		button.AddThemeStyleboxOverride("focus", newStyle);
	}

	//wenn bei einem ausgewöhlten charcter der "select"- button gedrückt wird
	private void _on_button_select_pressed()
	{
		if (_currentlySelectedCharacter != null)
		{
			string characterId = _currentlySelectedCharacter.Text.ToString();
			_characterManager.SaveLastSelectedCharacterID(int.Parse(characterId));


			SetButtonStyle(_currentlySelectedCharacter, Color.Color8(0x2C, 0xC7, 0xFF), true);

			if (_oldSelectedCharacter != _currentlySelectedCharacter)
			{
				_resetButton(_oldSelectedCharacter);
				_oldSelectedCharacter = _currentlySelectedCharacter;
			}
		}
		SoundManager.Instance.PlayUI();
	}

	// temp function for temp reset button to reset the character data 
	private void _resetButton(Button button)
	{
		if (button == null) return;
		var style = new StyleBoxFlat();
		style.BgColor = Color.Color8(0x4F, 0x4F, 0x4F);
		button.AddThemeStyleboxOverride("normal", style);
		button.AddThemeStyleboxOverride("hover", style);
		button.AddThemeStyleboxOverride("pressed", style);
		button.AddThemeStyleboxOverride("focus", style);
	}

	private void _on_button_upgrade_unlock_pressed()
	{
		var characterId = _currentlySelectedCharacter.Text;
		if (_characterManager.LoadIsUnlocked(characterId))
		{
			_characterManager.UpgradeCharacter(characterId);

			if (GameState.CurrentState == ConnectionState.Online)
			{
				var body = Json.Stringify(new Godot.Collections.Dictionary
				{
					{ "character_id", characterId }
				});

				var headers = new[]
				{
					"Content-Type: application/json",
					"Authorization: Bearer " + SecureStorage.LoadToken()
				};
				var err = _unlockRequest.Request(
					$"{Server.BaseUrl}/api/v1/protected/character/levelUp",
					headers,
					HttpClient.Method.Post,
					body
				);

				if (err != Error.Ok)
					GD.PrintErr($"AuthRequest error: {err}");
			}
		}
		else
		{
			_characterManager.SetUnlocked(characterId);
			_buttonUpgradeUnlock.Text = "Upgrade";
			_buttonSelect.Show();
			var icon = _currentlySelectedCharacter.GetNode<TextureRect>("TextureRect");
			icon.Material = null;

			GD.Print("unlcoked char local");
			if (GameState.CurrentState == ConnectionState.Online)
			{
				GD.Print("try to unlock char online");
				var body = Json.Stringify(new Godot.Collections.Dictionary
				{
					{ "character_id", characterId }
				});

				// Send POST request
				var headers = new[]
				{
					"Content-Type: application/json",
					"Authorization: Bearer " + SecureStorage.LoadToken()
				};
				var err = _unlockRequest.Request(
					$"{Server.BaseUrl}/api/v1/protected/character/unlock",
					headers,
					HttpClient.Method.Post,
					body
				);

				if (err != Error.Ok)
					GD.PrintErr($"AuthRequest error: {err}");
			}
		}

		SetCharacterPageValuesFromFile(characterId);
	    SoundManager.Instance.PlayUI();
	}

	private void ResetCharacters()
	{
		_characterManager.SaveCharacterData(1, "Archer", 100, 100, 100, 100, 1, 1);
		_characterManager.SaveCharacterData(2, "Assassin", 100, 100, 100, 100, 1, 0);
		_characterManager.SaveCharacterData(3, "Knight", 100, 100, 100, 100, 1, 0);
		_characterManager.SaveCharacterData(4, "Mage", 100, 100, 100, 100, 1, 0);

		Shader blackAndWhiteShader = GD.Load<Shader>("res://Menu/Character/characterMenuIconShader.gdshader");
		var material = new ShaderMaterial { Shader = blackAndWhiteShader };

		// Lock characters by applying the shader material
		LockCharacter("%Button_Character2", material); // Assassin
		LockCharacter("%Button_Character3", material); // Knight
		LockCharacter("%Button_Character4", material); // Mage

		// Unlock Archer
		UnlockCharacter("%Button_Character1");

		SetCharacterPageValuesFromFile(_currentlySelectedCharacter.Text);
	}

	/// <summary>
	/// Locks a character button by applying a shader material.
	/// </summary>
	private void LockCharacter(string buttonPath, ShaderMaterial material)
	{
		var button = GetNode<Button>(buttonPath);
		var icon = button.GetNode<TextureRect>("TextureRect");
		icon.Material = material;
	}

	/// <summary>
	/// Unlocks a character button by removing the shader material.
	/// </summary>
	private void UnlockCharacter(string buttonPath)
	{
		var button = GetNode<Button>(buttonPath);
		var icon = button.GetNode<TextureRect>("TextureRect");
		icon.Material = null;
	}

	/// <summary>
	/// Returns the character name based on the character ID.
	/// </summary>
	private string GetCharacterNameById()
	{
		string characterId = _currentlySelectedCharacter.Text.ToString();

		return characterId switch
		{
			"1" => "Archer",
			"2" => "Assassin",
			"3" => "Knight",
			"4" => "Mage",
			_ => "DefaultPlayer"
		};
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
}
