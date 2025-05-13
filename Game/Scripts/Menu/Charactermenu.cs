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
		SetCharacterPageValuesFromFile($"{selectedId}");


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
		
		SetCharacterPageValuesFromFile($"{selectedId}");
	}

	
	// set stats and name to values from file (see CharacterManager.cs)
	private void SetCharacterPageValuesFromFile(string characterId)
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
		SoundManager.Instance.PlayUI();
		GetTree().ChangeSceneToPacked(scene);
	}

	/// <summary>
	/// Handles character button selection. Updates the UI and checks if the character is unlocked.
	/// </summary>
	public void _characterSelected(ButtonsCharacterSelection button)
	{
		if (button != null && _labelCharacterName != null)
		{
			SetButtonStyle(_currentlySelectedCharacter, Color.Color8(0x4F, 0x4F, 0x4F), false);

			SetCharacterPageValuesFromFile(button.Text);
			_currentlySelectedCharacter = button;

			SetButtonStyle(_currentlySelectedCharacter, Color.Color8(0x2C, 0xC7, 0xFF), false);

			if (!characterManager.LoadIsUnlocked(button.Text))
			{
				_ButtonUpgradeUnlock.Text = "Unlock";
				_Button_Select.Hide();
			}
			else
			{
				_ButtonUpgradeUnlock.Text = "Upgrade";
				_Button_Select.Show();
			}

			GD.Print($"[Charactermenu] Selected Character: {GetCharacterNameById()}");
		}
	}
	
	private void SetButtonStyle(Button button, Color color, bool addBorder){
		var style = button.GetThemeStylebox("normal") as StyleBoxFlat;
		if(style!=null){
			
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
	}

	/// <summary>
	/// Handles the "Select" button press for the currently selected character.
	/// Saves the selected character ID and updates button styles.
	/// </summary>
	private void _on_button_select_pressed()
	{
		if (_currentlySelectedCharacter != null)
		{
			string characterId = _currentlySelectedCharacter.Text.ToString();
			characterManager.SaveLastSelectedCharacterID(int.Parse(characterId));
			GD.Print($"[Charactermenu] Selected Character ID: {characterId}");

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
		if (button != null)
		{
			var style = new StyleBoxFlat();
			style.BgColor = Color.Color8(0x4F, 0x4F, 0x4F);
			button.AddThemeStyleboxOverride("normal", style);
			button.AddThemeStyleboxOverride("hover", style);
			button.AddThemeStyleboxOverride("pressed", style);
			button.AddThemeStyleboxOverride("focus", style);
		}
	}
	
	private void _on_button_upgrade_unlock_pressed()
	{
		string characterID=_currentlySelectedCharacter.Text;
		if(characterManager.LoadIsUnlocked(characterID))
		{
			characterManager.UpgradeCharacter(characterID);
			
		}else
		{
			characterManager.SetUnlocked(characterID);
			_ButtonUpgradeUnlock.Text = "Upgrade";
			_Button_Select.Show();
			var icon=_currentlySelectedCharacter.GetNode<TextureRect>("TextureRect");
			icon.Material=null;
		}
		SetCharacterPageValuesFromFile(characterID);
		SoundManager.Instance.PlayUI();
	}
	
	/// <summary>
	/// Resets character data and locks characters that are not unlocked.
	/// </summary>
	private void ResetCharacters()
	{
		characterManager.SaveCharacterData(1, "Archer", 100, 100, 100, 100, 1, 1);
		characterManager.SaveCharacterData(2, "Assassin", 100, 100, 100, 100, 1, 0);
		characterManager.SaveCharacterData(3, "Knight", 100, 100, 100, 100, 1, 0);
		characterManager.SaveCharacterData(4, "Mage", 100, 100, 100, 100, 1, 0);

		Shader blackAndWhiteShader = GD.Load<Shader>("res://Scenes/Menu/characterMenuIconShader.gdshader");
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
}
