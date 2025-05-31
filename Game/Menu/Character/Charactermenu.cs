using System.Text;
using Game.Utilities.Backend;
using Godot;
using Godot.Collections;

namespace Game.Menu.Character;

public partial class Charactermenu : Control
{
	private const int UpgradeCost = 50;
	private const int UnlockCost = 100;

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
	private Label _goldLabel;

	private HttpRequest _unlockRequest;
	private HttpRequest _levelUpRequest;
	private HttpRequest _progressCheckRequest;
	private HttpRequest _setGoldRequest;

	public override void _Ready()
	{
		_characterManager = GetNode<CharacterManager>("/root/CharacterManager");

		_labelHealth = GetNode<Label>("%Label_health");
		_labelSpeed = GetNode<Label>("%Label_speed");
		_labelDexterity = GetNode<Label>("%Label_dexterity");
		_labelIntelligence = GetNode<Label>("%Label_intelligence");
		_labelCharacterName = GetNode<Label>("%Label_SelectedCharacterName");
		_goldLabel = GetNode<Label>("%Gold");

		_buttonUpgradeUnlock = GetNode<Button>("%Button_UpgradeUnlock");
		_buttonSelect = GetNode<Button>("%Button_Select");

		_unlockRequest = GetNode<HttpRequest>("%UnlockRequest");
		_levelUpRequest = GetNode<HttpRequest>("%LevelUpRequest");
		_setGoldRequest = GetNode<HttpRequest>("%SetGoldRequest");

		_unlockRequest.Connect("request_completed", new Callable(this, nameof(OnUnlockRequestCompleted)));
		_levelUpRequest.Connect("request_completed", new Callable(this, nameof(OnLevelUpRequestCompleted)));
		_setGoldRequest.Connect("request_completed", new Callable(this, nameof(OnSetGoldRequestCompleted)));

		var selectedId = _characterManager.LoadLastSelectedCharacterID();
		_currentlySelectedCharacter = GetNode<Button>($"%Button_Character{selectedId}");
		_on_button_select_pressed();
		SetCharacterPageValuesFromFile($"{selectedId}");
		UpdateGoldLabel();

		if (GameState.CurrentState != ConnectionState.Online) return;
		var headers = new[]
		{
			"Content-Type: application/json",
			"Authorization: Bearer " + SecureStorage.LoadToken()
		};
		var err = _progressCheckRequest.Request(
			$"{ServerConfig.BaseUrl}/api/v1/protected/progress",
			headers,
			HttpClient.Method.Post
		);

		if (err != Error.Ok)
			GD.PrintErr($"AuthRequest error: {err}");
	}

	private void OnSetGoldRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		var receivedString = Encoding.UTF8.GetString(body);
		GD.Print("Antwort empfangen: " + receivedString);

		GD.Print(responseCode == 200 ? "Gold successfully set" : "Error setting gold");
	}

	private void OnLevelUpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		var receivedString = Encoding.UTF8.GetString(body);
		GD.Print("Antwort empfangen: " + receivedString);

		GD.Print(responseCode == 200 ? "Character unlocked" : "Error unlocking character");
	}

	private void OnUnlockRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		var receivedString = Encoding.UTF8.GetString(body);
		GD.Print("Antwort empfangen: " + receivedString);

		GD.Print(responseCode == 200 ? "Character unlocked" : "Error unlocking character");
	}

	private void SetCharacterPageValuesFromFile(string characterId)
	{
		_labelCharacterName.Text =
			$"{_characterManager.LoadNameByID(characterId)} - Lvl.{_characterManager.LoadLevelByID(characterId)}";
		_labelHealth.Text = $"Health {_characterManager.LoadHealthByID(characterId)}";
		_labelSpeed.Text = $"Speed {_characterManager.LoadSpeedByID(characterId)}";
		_labelDexterity.Text = $"Dexterity {_characterManager.LoadDexterityByID(characterId)}";
		_labelIntelligence.Text = $"Intelligence {_characterManager.LoadIntelligenceByID(characterId)}";
		UpdateUpgradeUnlockButtonState();
		UpdateGoldLabel();
	}

	private void _on_button_back_charactermenu_pressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Menu/Main/mainMenu.tscn");
		SoundManager.Instance.PlayUI();
		GetTree().ChangeSceneToPacked(scene);
	}

	public void _characterSelected(ButtonsCharacterSelection button)
	{
		if (button == null || _labelCharacterName == null) return;
		SetButtonStyle(_currentlySelectedCharacter, Color.Color8(0x4F, 0x4F, 0x4F), false);

		SetCharacterPageValuesFromFile(button.Text);
		_currentlySelectedCharacter = button;

		SetButtonStyle(_currentlySelectedCharacter, Color.Color8(0x2C, 0xC7, 0xFF), false);

		if (!_characterManager.LoadIsUnlocked(button.Text))
		{
			_buttonUpgradeUnlock.Text = "Unlock";
			_buttonSelect.Hide();
		}
		else
		{
			_buttonUpgradeUnlock.Text = "Upgrade";
			_buttonSelect.Show();
		}
		UpdateUpgradeUnlockButtonState();
		UpdateGoldLabel();
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

	private void UpdateUpgradeUnlockButtonState()
	{
		var characterId = _currentlySelectedCharacter.Text;
		var isUnlocked = _characterManager.LoadIsUnlocked(characterId);
		var gold = _characterManager.LoadGold();
		var canAfford = isUnlocked ? gold >= UpgradeCost : gold >= UnlockCost;

		_buttonUpgradeUnlock.Disabled = !canAfford;

		var style = _buttonUpgradeUnlock.GetThemeStylebox("normal") as StyleBoxFlat;
		if (style == null) return;
		var newStyle = (StyleBoxFlat)style.Duplicate();
		newStyle.BgColor = canAfford ? Color.Color8(0x2C, 0xC7, 0xFF) : Color.Color8(0x80, 0x80, 0x80);
		_buttonUpgradeUnlock.AddThemeStyleboxOverride("normal", newStyle);
		_buttonUpgradeUnlock.AddThemeStyleboxOverride("hover", newStyle);
		_buttonUpgradeUnlock.AddThemeStyleboxOverride("pressed", newStyle);
		_buttonUpgradeUnlock.AddThemeStyleboxOverride("focus", newStyle);
	}

	private void UpdateGoldLabel()
	{
		if (_goldLabel != null)
			_goldLabel.Text = $"Gold: {_characterManager.LoadGold()}";
	}

	private void _on_button_select_pressed()
	{
		var characterId = _currentlySelectedCharacter.Text;
		if (_currentlySelectedCharacter != null)
		{
			_characterManager.SaveLastSelectedCharacterID(int.Parse(characterId));

			SetButtonStyle(_currentlySelectedCharacter, Color.Color8(0x2C, 0xC7, 0xFF), true);

			if (_oldSelectedCharacter != _currentlySelectedCharacter)
			{
				_resetButton(_oldSelectedCharacter);
				_oldSelectedCharacter = _currentlySelectedCharacter;
			}
		}
		SoundManager.Instance.PlayUI();
		UpdateUpgradeUnlockButtonState();
		UpdateGoldLabel();
	}

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
			if (_characterManager.LoadGold() >= UpgradeCost)
			{
				_characterManager.SubGold(UpgradeCost);
				SetGoldOnline();
				UpgradeCharacterOnline(characterId);
			}
			else
			{
				GD.Print("Nicht genug Gold zum Upgraden!");
			}
		}
		else
		{
			if (_characterManager.LoadGold() >= UnlockCost)
			{
				_characterManager.SubGold(UnlockCost);
				SetGoldOnline();
				UnlockCharacterOnline(characterId);
			}
			else
			{
				GD.Print("Nicht genug Gold zum Freischalten!");
			}
		}

		SetCharacterPageValuesFromFile(characterId);
		SoundManager.Instance.PlayUI();
		UpdateUpgradeUnlockButtonState();
		UpdateGoldLabel();
	}

	private void SetGoldOnline()
	{
		if (GameState.CurrentState != ConnectionState.Online) return;

		var gold = _characterManager.LoadGold();
		var body = Json.Stringify(new Dictionary
		{
			{ "gold", gold }
		});

		var headers = new[]
		{
			"Content-Type: application/json",
			"Authorization: Bearer " + SecureStorage.LoadToken()
		};
		var err = _setGoldRequest.Request(
			$"{ServerConfig.BaseUrl}/api/v1/protected/gold/set",
			headers,
			HttpClient.Method.Post,
			body
		);

		if (err != Error.Ok)
			GD.PrintErr($"AuthRequest error: {err}");
	}

	private void UpgradeCharacterOnline(string characterId)
	{
		_characterManager.UpgradeCharacter(characterId);

		if (GameState.CurrentState == ConnectionState.Online)
		{
			var body = Json.Stringify(new Dictionary
			{
				{ "character_id", characterId }
			});

			var headers = new[]
			{
				"Content-Type: application/json",
				"Authorization: Bearer " + SecureStorage.LoadToken()
			};
			var err = _unlockRequest.Request(
				$"{ServerConfig.BaseUrl}/api/v1/protected/character/levelUp",
				headers,
				HttpClient.Method.Post,
				body
			);

			if (err != Error.Ok)
				GD.PrintErr($"AuthRequest error: {err}");
		}
	}

	private void UnlockCharacterOnline(string characterId)
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
			var body = Json.Stringify(new Dictionary
			{
				{ "character_id", characterId }
			});

			var headers = new[]
			{
				"Content-Type: application/json",
				"Authorization: Bearer " + SecureStorage.LoadToken()
			};
			var err = _unlockRequest.Request(
				$"{ServerConfig.BaseUrl}/api/v1/protected/character/unlock",
				headers,
				HttpClient.Method.Post,
				body
			);

			if (err != Error.Ok)
				GD.PrintErr($"AuthRequest error: {err}");
		}
	}

	private void ResetCharacters()
	{
		_characterManager.SaveCharacterData(1, "Archer", 100, 100, 100, 100, 1, 1);
		_characterManager.SaveCharacterData(2, "Assassin", 100, 100, 100, 100, 1, 0);
		_characterManager.SaveCharacterData(3, "Knight", 100, 100, 100, 100, 1, 0);
		_characterManager.SaveCharacterData(4, "Mage", 100, 100, 100, 100, 1, 0);

		var blackAndWhiteShader = GD.Load<Shader>("res://Menu/Character/characterMenuIconShader.gdshader");
		var material = new ShaderMaterial { Shader = blackAndWhiteShader };

		LockCharacter("%Button_Character2", material);
		LockCharacter("%Button_Character3", material);
		LockCharacter("%Button_Character4", material);

		UnlockCharacter("%Button_Character1");

		SetCharacterPageValuesFromFile(_currentlySelectedCharacter.Text);
		UpdateGoldLabel();
	}

	private void LockCharacter(string buttonPath, ShaderMaterial material)
	{
		var button = GetNode<Button>(buttonPath);
		var icon = button.GetNode<TextureRect>("TextureRect");
		icon.Material = material;
	}

	private void UnlockCharacter(string buttonPath)
	{
		var button = GetNode<Button>(buttonPath);
		var icon = button.GetNode<TextureRect>("TextureRect");
		icon.Material = null;
	}

	private string GetCharacterNameById()
	{
		var characterId = _currentlySelectedCharacter.Text;

		return characterId switch
		{
			"1" => "Archer",
			"2" => "Assassin",
			"3" => "Knight",
			"4" => "Mage",
			_ => "DefaultPlayer"
		};
	}
}
