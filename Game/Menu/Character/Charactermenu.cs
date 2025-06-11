using System.Diagnostics;
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
	private Button _buttonAbilityOne;
	private Button _buttonAbilityTwo;

	private Label _labelHealth;
	private Label _labelSpeed;
	private Label _labelStrength;
	private Label _labelDexterity;
	private Label _labelIntelligence;
	private Label _labelAbilityOne;
	private Label _labelAbilityTwo;
	private Label _goldLabel;

	private Label _labelHealthUpgrade;
	private Label _labelSpeedUpgrade;
	private Label _labelStrengthUpgrade;
	private Label _labelDexterityUpgrade;
	private Label _labelIntelligenceUpgrade;

	private HttpRequest _unlockRequest;
	private HttpRequest _levelUpRequest;
	private HttpRequest _progressCheckRequest;
	private HttpRequest _setGoldRequest;
	private int _maxLevel = 25;

	public override void _Ready()
	{
		_characterManager = GetNode<CharacterManager>("/root/CharacterManager");

		_labelHealth = GetNode<Label>("%Label_health");
		_labelSpeed = GetNode<Label>("%Label_speed");
		_labelStrength = GetNode<Label>("%Label_strength");
		_labelDexterity = GetNode<Label>("%Label_dexterity");
		_labelIntelligence = GetNode<Label>("%Label_intelligence");

		_labelHealthUpgrade = GetNode<Label>("%Label_HealthUpgrade");
		_labelSpeedUpgrade = GetNode<Label>("%Label_SpeedUpgrade");
		_labelStrengthUpgrade = GetNode<Label>("%Label_StrengthUpgrade");
		_labelDexterityUpgrade = GetNode<Label>("%Label_DexterityUpgrade");
		_labelIntelligenceUpgrade = GetNode<Label>("%Label_IntelligenceUpgrade");

		_labelCharacterName = GetNode<Label>("%Label_SelectedCharacterName");
		_goldLabel = GetNode<Label>("%Gold");

		_buttonUpgradeUnlock = GetNode<Button>("%Button_UpgradeUnlock");
		_buttonSelect = GetNode<Button>("%Button_Select");

		_buttonUpgradeUnlock.Text = "Upgrade: " + UpgradeCost + " Gold";

		_labelAbilityOne = GetNode<Label>("%Label_Ability1");
		_labelAbilityTwo = GetNode<Label>("%Label_Ability2");
		_buttonAbilityOne = GetNode<Button>("%Button_Ability1");
		_buttonAbilityOne = GetNode<Button>("%Button_Ability2");

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

		GD.Print(responseCode == 200 ? "Gold erfolgreich gesetzt" : "Fehler beim Setzen von Gold");
	}

	private void OnLevelUpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		var receivedString = Encoding.UTF8.GetString(body);
		GD.Print("Antwort empfangen: " + receivedString);

		GD.Print(responseCode == 200 ? "Charakter gelevelt" : "Fehler beim Leveln des Charakters");
	}

	private void OnUnlockRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		var receivedString = Encoding.UTF8.GetString(body);
		GD.Print("Antwort empfangen: " + receivedString);

		GD.Print(responseCode == 200 ? "Charakter freigeschaltet" : "Fehler beim Freischalten des Charakters");
	}

	private void SetCharacterPageValuesFromFile(string characterId)
	{
		_labelCharacterName.Text =
			$"{_characterManager.LoadNameByID(characterId)} - Lvl.{_characterManager.LoadLevelByID(characterId)}";
		_labelHealth.Text = $"Health {_characterManager.LoadHealthByID(characterId)}";
		_labelSpeed.Text = $"Speed {_characterManager.LoadSpeedByID(characterId)}";
		_labelStrength.Text = $"Strength {_characterManager.LoadStrengthByID(characterId)}";
		_labelDexterity.Text = $"Dexterity {_characterManager.LoadDexterityByID(characterId)}";
		_labelIntelligence.Text = $"Intelligence {_characterManager.LoadIntelligenceByID(characterId)}";
		_labelAbilityOne.Text = $"{_characterManager.LoadAbilityOneByID(characterId)}";
		_labelAbilityTwo.Text = $"{_characterManager.LoadAbilityTwoByID(characterId)}";

		UpdateUpgradeUnlockButtonState();
		UpdateGoldLabel();

		if (_characterManager.LoadLevelByID(characterId) < _maxLevel)
		{
			var upgradeAmount = _characterManager.GetUpgradeAmount(_characterManager.LoadLevelByID(characterId));
			_labelHealthUpgrade.Text = $"   +{upgradeAmount}";
			_labelSpeedUpgrade.Text = $"   +{upgradeAmount}";
			_labelStrengthUpgrade.Text = $"   +{upgradeAmount}";
			_labelDexterityUpgrade.Text = $"   +{upgradeAmount}";
			_labelIntelligenceUpgrade.Text = $"   +{upgradeAmount}";
		}
	}

	private void _on_button_back_charactermenu_pressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Menu/Main/mainMenu.tscn");
		SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
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
			_buttonUpgradeUnlock.Text = "Unlock: " + UnlockCost + " Gold";
			_buttonSelect.Hide();
		}
		else
		{
			_buttonUpgradeUnlock.Text = "Upgrade: " + UpgradeCost + " Gold";
			_buttonSelect.Show();
		}

		UpdateUpgradeUnlockButtonState();
		UpdateGoldLabel();

		if (_characterManager.LoadLevelByID(button.Text) < _maxLevel)
			_buttonUpgradeUnlock.Show();
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

		SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
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
			HandleUpgrade(characterId);
		}
		else
		{
			HandleUnlock(characterId);
		}

		UpdateCharacterUi(characterId);
		SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
	}

	private void _on_button_ability_1_pressed()
	{
		var characterId = _currentlySelectedCharacter.Text;
		_characterManager.SetAbilityByID(characterId, 1);
		SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
	}

	private void _on_button_ability_2_pressed()
	{
		var characterId = _currentlySelectedCharacter.Text;
		_characterManager.SetAbilityByID(characterId, 2);
		SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
	}

	private void _on_button_abilities_pressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Menu/Ability/ability_menu.tscn");
		SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
		GetTree().ChangeSceneToPacked(scene);
	}

	// --- Refaktorisierte Flows ---

	private void HandleUpgrade(string characterId)
	{
		if (!CanUpgrade(characterId))
		{
			ClearUpgradeLabelsAndHideButton();
			return;
		}

		PerformUpgrade(characterId);
		UpdateServerAfterUpgrade(characterId);

		if (_characterManager.LoadLevelByID(characterId) == _maxLevel)
			ClearUpgradeLabelsAndHideButton();
	}

	private void HandleUnlock(string characterId)
	{
		if (!CanUnlock())
			return;

		PerformUnlock(characterId);
		UpdateServerAfterUnlock(characterId);
	}

	// --- Einzelverantwortlichkeiten ---

	private bool CanUpgrade(string characterId)
	{
		return _characterManager.LoadLevelByID(characterId) < _maxLevel && _characterManager.LoadGold() >= UpgradeCost;
	}

	private bool CanUnlock()
	{
		return _characterManager.LoadGold() >= UnlockCost;
	}

	private void PerformUpgrade(string characterId)
	{
		_characterManager.SubGold(UpgradeCost);
		_characterManager.UpgradeCharacter(characterId);
	}

	private void PerformUnlock(string characterId)
	{
		_characterManager.SubGold(UnlockCost);
		_characterManager.SetUnlocked(characterId);
		_buttonUpgradeUnlock.Text = "Upgrade";
		_buttonSelect.Show();
		var icon = _currentlySelectedCharacter.GetNode<TextureRect>("TextureRect");
		icon.Material = null;
		GD.Print("unlcoked char local");
	}

	private void UpdateServerAfterUpgrade(string characterId)
	{
		if (GameState.CurrentState == ConnectionState.Online)
		{
			SendUpgradeRequest(characterId);
			SendGoldUpdateRequest();
		}
	}

	private void UpdateServerAfterUnlock(string characterId)
	{
		if (GameState.CurrentState == ConnectionState.Online)
		{
			SendUnlockRequest(characterId);
			SendGoldUpdateRequest();
		}
	}

	private void UpdateCharacterUi(string characterId)
	{
		SetCharacterPageValuesFromFile(characterId);
	}

	private void ClearUpgradeLabelsAndHideButton()
	{
		_buttonUpgradeUnlock.Hide();
		_labelHealthUpgrade.Text = "";
		_labelSpeedUpgrade.Text = "";
		_labelStrengthUpgrade.Text = "";
		_labelDexterityUpgrade.Text = "";
		_labelIntelligenceUpgrade.Text = "";
	}

	// --- Server Requests ---

	private void SendUpgradeRequest(string characterId)
	{
		var body = Json.Stringify(new Dictionary { { "character_id", characterId } });
		var headers = GetAuthHeaders();
		var err = _unlockRequest.Request(
			$"{ServerConfig.BaseUrl}/api/v1/protected/character/levelUp",
			headers,
			HttpClient.Method.Post,
			body
		);
		if (err != Error.Ok)
			GD.PrintErr($"AuthRequest error: {err}");
	}

	private void SendUnlockRequest(string characterId)
	{
		GD.Print("try to unlock char online");
		var body = Json.Stringify(new Dictionary { { "character_id", characterId } });
		var headers = GetAuthHeaders();
		var err = _unlockRequest.Request(
			$"{ServerConfig.BaseUrl}/api/v1/protected/character/unlock",
			headers,
			HttpClient.Method.Post,
			body
		);
		if (err != Error.Ok)
			GD.PrintErr($"AuthRequest error: {err}");
	}

	private void SendGoldUpdateRequest()
	{
		var body = Json.Stringify(new Dictionary { { "gold", _characterManager.LoadGold() } });
		var headers = GetAuthHeaders();
		var err = _setGoldRequest.Request(
			$"{ServerConfig.BaseUrl}/api/v1/protected/gold/set",
			headers,
			HttpClient.Method.Post,
			body
		);
		if (err != Error.Ok)
			GD.PrintErr($"SetGoldRequest error: {err}");
	}

	private static string[] GetAuthHeaders()
	{
		return
		[
			"Content-Type: application/json",
			"Authorization: Bearer " + SecureStorage.LoadToken()
		];
	}

	private void ResetCharacters()
	{
		_characterManager.SaveCharacterData(1, "Archer", 85, 200, 100, 100, 110, 1, "Boost Dexterity", "Arrow Rain", 1, 1);
		_characterManager.SaveCharacterData(2, "Assassin", 70, 220, 100, 100, 110, 1, "Dash", "Deadly Strike", 1, 0);
		_characterManager.SaveCharacterData(3, "Knight", 125, 180, 100, 125,  85, 1, "Boost Strength", "Fortress", 1, 0);
		_characterManager.SaveCharacterData(4, "Mage", 100, 200, 100, 110, 110, 1, "Boost Intelligence", "Beam of Destruction", 1, 0);
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
	
	public override void _Notification(int what)
	{
		if (what == NotificationWMGoBackRequest)
		{
			_on_button_back_charactermenu_pressed();
		}   
	}
}
