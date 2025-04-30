using Godot;
using System;
using System.Text;
using System.Collections.Generic;


public partial class Charactermenu : Control
{
	private CharacterManager characterManager;
	private Label _labelCharacterName;
	private Button _currentlySelectedCharacter;
	private Button _oldSelectedCharacter;
	private Button _ButtonUpgradeUnlock;
	private Button _Button_Select;

	private Label _labelHealth;
	private Label _labelSpeed;
	private Label _labelDexterity;
	private Label _labelIntelligence;
	
	public override void _Ready()
	{
		characterManager = GetNode<CharacterManager>("/root/CharacterManager");
		GD.Print(characterManager.LoadLastSelectedCharacterID());

		_labelHealth = GetNode<Label>("%Label_health");
		_labelSpeed = GetNode<Label>("%Label_speed");
		_labelDexterity = GetNode<Label>("%Label_dexterity");
		_labelIntelligence = GetNode<Label>("%Label_intelligence");

		_labelCharacterName = GetNode<Label>("%Label_SelectedCharacterName");
		_ButtonUpgradeUnlock = GetNode<Button>("%Button_UpgradeUnlock");
		_Button_Select = GetNode<Button>("%Button_Select");

		HttpRequest httpRequest = GetNode<HttpRequest>("%HTTPRequest");
		httpRequest.RequestCompleted += OnRequestCompleted;

		string jsonData = "{\"user_id\": \"D465D2FD-092C-40A6-945D-E29E99FA524A\"}";
		Error error = httpRequest.Request("http://localhost:3000/api/v1/protected/getAllCharacters", new string[] { "Content-Type: application/json" }, HttpClient.Method.Post, jsonData);
		if (error != Error.Ok)
		{
			GD.Print("Request nicht gesendet");
		}
		
		int selectedId = characterManager.LoadLastSelectedCharacterID();
		_currentlySelectedCharacter = GetNode<Button>($"%Button_Character{selectedId}");
		_on_button_select_pressed();
		SetCharacterPageValuesFromFile(selectedId);


	}
	private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		string receivedString = Encoding.UTF8.GetString(body);
		GD.Print("Antwort empfangen: " + receivedString);
		Variant receivedVar = Json.ParseString(receivedString);
		//allCharacters = receivedVar.AsGodotArray();


		int selectedId = characterManager.LoadLastSelectedCharacterID();
		_currentlySelectedCharacter = GetNode<Button>($"%Button_Character{selectedId}");
		_on_button_select_pressed();
		
		SetCharacterPageValuesFromFile(selectedId);
	}

	
	// set stats and name to values from file (see CharacterManager.cs)
	private void SetCharacterPageValuesFromFile(int characterId)
	{
			_labelCharacterName.Text = $"{characterManager.LoadNameByID(characterId)} - Lvl.{characterManager.LoadLevelByID(characterId)}";
			_labelHealth.Text = $"Health {characterManager.LoadHealthByID(characterId)}";
			_labelSpeed.Text = $"Speed {characterManager.LoadSpeedByID(characterId)}";
			_labelDexterity.Text = $"Dexterity {characterManager.LoadDexterityByID(characterId)}";
			_labelIntelligence.Text = $"Intelligence {characterManager.LoadIntelligenceByID(characterId)}";
	}


	private void _on_button_back_charactermenu_pressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/mainMenu.tscn");
		GetTree().ChangeSceneToPacked(scene);
	}

	//wenn ein character button angeklickt wird
	public void _characterSelected(ButtonsCharacterSelection button)
	{
		if (button != null && _labelCharacterName != null)
		{
			SetCharacterPageValuesFromFile(int.Parse(button.Text));
			_currentlySelectedCharacter = button;
			if (!characterManager.LoadIsUnlocked(int.Parse(button.Text)))
			{ //check if character is unlocken. locked=true
				_ButtonUpgradeUnlock.Text = "Unlock";
				_Button_Select.Disabled = true;
				_Button_Select.Hide();
			}
			else
			{
				_ButtonUpgradeUnlock.Text = "Upgrade";
				_Button_Select.Disabled = false;
				_Button_Select.Show();
			}
		}
	}

	//wenn bei einem ausgewöhlten charcter der "select"- button gedrückt wird 
	private void _on_button_select_pressed()
	{
		if (_currentlySelectedCharacter != null)
		{
			characterManager.SaveLastSelectedCharacterID(int.Parse(_currentlySelectedCharacter.Text));
			var style = new StyleBoxFlat();
			style.BgColor = Color.Color8(0x4F, 0x4F, 0x4F);
			style.BorderColor = new Color(1f, 0f, 0f);
			style.SetBorderWidth(Side.Top, 3);
			style.SetBorderWidth(Side.Bottom, 3);
			style.SetBorderWidth(Side.Left, 3);
			style.SetBorderWidth(Side.Right, 3);
			_currentlySelectedCharacter.AddThemeStyleboxOverride("normal", style);

			if (_oldSelectedCharacter != _currentlySelectedCharacter)
			{
				_resetButton(_oldSelectedCharacter);
				_oldSelectedCharacter = _currentlySelectedCharacter;
			}
		}
	}
	private void _resetButton(Button button)
	{
		if (button != null)
		{
			var style = new StyleBoxFlat();
			style.BgColor = Color.Color8(0x4F, 0x4F, 0x4F);
			button.AddThemeStyleboxOverride("normal", style);
		}
	}



}
