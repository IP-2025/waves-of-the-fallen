using System.Text;
using Game.Scripts.Config;
using Godot;

namespace Game.Scripts.Menu;

public partial class CharacterMenu : Control
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

	private HttpRequest _httpRequest;
	
	public override void _Ready()
	{
		_characterManager = GetNode<CharacterManager>("/root/CharacterManager");

		_labelHealth = GetNode<Label>("%Label_health");
		_labelSpeed = GetNode<Label>("%Label_speed");
		_labelDexterity = GetNode<Label>("%Label_dexterity");
		_labelIntelligence = GetNode<Label>("%Label_intelligence");

		_labelCharacterName = GetNode<Label>("%Label_SelectedCharacterName");
		_buttonUpgradeUnlock = GetNode<Button>("%Button_UpgradeUnlock");
		_buttonSelect = GetNode<Button>("%Button_Select");

		_httpRequest = GetNode<HttpRequest>("%HTTPRequest");
		_httpRequest.Connect("request_completed", new Callable(this, nameof(OnRequestCompleted)));

		var selectedId = _characterManager.LoadLastSelectedCharacterID();
		_currentlySelectedCharacter = GetNode<Button>($"%Button_Character{selectedId}");
		_on_button_select_pressed();
		SetCharacterPageValuesFromFile($"{selectedId}");


	}
	private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		var receivedString = Encoding.UTF8.GetString(body);
		GD.Print("Antwort empfangen: " + receivedString);
		var receivedVar = Json.ParseString(receivedString);
		//allCharacters = receivedVar.AsGodotArray();


		var selectedId = _characterManager.LoadLastSelectedCharacterID();
		_currentlySelectedCharacter = GetNode<Button>($"%Button_Character{selectedId}");
		_on_button_select_pressed();
		
		SetCharacterPageValuesFromFile($"{selectedId}");
	}

	
	// set stats and name to values from file (see CharacterManager.cs)
	private void SetCharacterPageValuesFromFile(string characterId)
	{
		_labelCharacterName.Text = $"{_characterManager.LoadNameByID(characterId)} - Lvl.{_characterManager.LoadLevelByID(characterId)}";
		_labelHealth.Text = $"Health {_characterManager.LoadHealthByID(characterId)}";
		_labelSpeed.Text = $"Speed {_characterManager.LoadSpeedByID(characterId)}";
		_labelDexterity.Text = $"Dexterity {_characterManager.LoadDexterityByID(characterId)}";
		_labelIntelligence.Text = $"Intelligence {_characterManager.LoadIntelligenceByID(characterId)}";
	}


	private void _on_button_back_charactermenu_pressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/mainMenu.tscn");
		GetTree().ChangeSceneToPacked(scene);
	}

	//wenn ein character button angeklickt wird
	public void _characterSelected(ButtonsCharacterSelection button)
	{
		if (button == null || _labelCharacterName == null) return;
		SetButtonStyle(_currentlySelectedCharacter,Color.Color8(0x4F, 0x4F, 0x4F),false);
			
		SetCharacterPageValuesFromFile(button.Text);
		_currentlySelectedCharacter = button;
			
		SetButtonStyle(_currentlySelectedCharacter,Color.Color8(0x2C, 0xC7, 0xFF),false);
			
		if (!_characterManager.LoadIsUnlocked(button.Text))
		{ //check if character is unlocken. locked=true
			_buttonUpgradeUnlock.Text = "Unlock";
			_buttonSelect.Hide();
		}
		else
		{
			_buttonUpgradeUnlock.Text = "Upgrade";
			_buttonSelect.Show();
		}
	}
	
	private void SetButtonStyle(Button button, Color color, bool addBorder){
		var style = button.GetThemeStylebox("normal") as StyleBoxFlat;
		if (style == null) return;
		var newStyle=(StyleBoxFlat)style.Duplicate();
		newStyle.BgColor=color;
		newStyle.BorderColor = new Color(1f, 0f, 0f);
			
		if(addBorder){
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
		if (_currentlySelectedCharacter == null) return;
		_characterManager.SaveLastSelectedCharacterID(int.Parse(_currentlySelectedCharacter.Text));
		SetButtonStyle(_currentlySelectedCharacter, Color.Color8(0x2C, 0xC7, 0xFF),true);

		if (_oldSelectedCharacter == _currentlySelectedCharacter) return;
		_resetButton(_oldSelectedCharacter);
		_oldSelectedCharacter = _currentlySelectedCharacter;
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
	
	// TODO: need to work here
	private void _on_button_upgrade_unlock_pressed()
	{
		var characterId =_currentlySelectedCharacter.Text;
		if(_characterManager.LoadIsUnlocked(characterId))
		{
			_characterManager.UpgradeCharacter(characterId);
			
			if (GameState.CurrentState == ConnectionState.Online)
			{
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
				var err = _httpRequest.Request(
					$"{Server.BaseUrl}/api/v1/protected/character/unlock",
					headers,
					HttpClient.Method.Post,
					body
				);
				
				if (err != Error.Ok)
					GD.PrintErr($"AuthRequest error: {err}");
			}
			
		}else
		{
			_characterManager.SetUnlocked(characterId);
			_buttonUpgradeUnlock.Text = "Upgrade";
			_buttonSelect.Show();
			var icon=_currentlySelectedCharacter.GetNode<TextureRect>("TextureRect");
			icon.Material=null;
		}
		SetCharacterPageValuesFromFile(characterId);

	}
	
	private void ResetCharacters()
	{
		ConfigFile config = new();
		config.Load("user://character_settings.cfg");
		
		_characterManager.SaveCharacterData(1, "Archer", 100, 100, 100, 100, 1, 1);
		_characterManager.SaveCharacterData(2, "Assassin", 100, 100, 100, 100, 1, 0);
		_characterManager.SaveCharacterData(3, "Knight", 100, 100, 100, 100, 1, 0);
		_characterManager.SaveCharacterData(4, "Mage", 100, 100, 100, 100, 1, 0);
		
		var button1 = GetNode<Button>("%Button_Character1");
		var button2 = GetNode<Button>("%Button_Character2");
		var button3 = GetNode<Button>("%Button_Character3");
		var button4 = GetNode<Button>("%Button_Character4");
		
		var icon=button1.GetNode<TextureRect>("TextureRect");
		icon.Material=null;
		
		var blackAndWhiteShader = GD.Load<Shader>("res://Scenes/Menu/characterMenuIconShader.gdshader");
		var material = new ShaderMaterial();
		material.Shader = blackAndWhiteShader;
		
		icon=button2.GetNode<TextureRect>("TextureRect");
		icon.Material=material;
		
		icon=button3.GetNode<TextureRect>("TextureRect");
		icon.Material=material;
		
		icon=button4.GetNode<TextureRect>("TextureRect");
		icon.Material=material;
		
		
		SetCharacterPageValuesFromFile(_currentlySelectedCharacter.Text);
	}
}