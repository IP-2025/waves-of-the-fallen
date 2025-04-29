using Godot;
using System;
using System.Text;
using System.Collections.Generic;


public partial class Charactermenu : Control
{
	private Label _labelCharacterName;
	private Button _currentlySelectedCharacter;
	private Button _oldSelectedCharacter;
	private Button _ButtonUpgradeUnlock;
	private Button _Button_Select;
	//private Godot.Collections.Array allCharacters;
	
	private Label _labelHealth;
	private Label _labelSpeed;
	private Label _labelDexterity;
	private Label _labelIntelligence;
	//das array wird hier mal temporär mit daten befüllt, falls die DB abfrage nicht geht
	//generell sollten die Character stats nicht jedes mal abgefragt werden, da man dafür immer eine INternet verbingung braucht
	//alos am besten Daten wie welche CHractere gibt es und welche sind auf welchen level freigeschaltet lokal speichern und nurm it internetverbindung aktualisieren
private Godot.Collections.Array allCharacters = new Godot.Collections.Array
	{
		new Godot.Collections.Dictionary
		{
			{"character_id", 1},
			{"name", "Archer"},
			{"speed", 100},
			{"health", 100},
			{"dexterity", 100},
			{"intelligence", 100}
		},
		new Godot.Collections.Dictionary
		{
			{"character_id", 2},
			{"name", "Assassin"},
			{"speed", 100},
			{"health", 100},
			{"dexterity", 100},
			{"intelligence", 100}
		},
		new Godot.Collections.Dictionary
		{
			{"character_id", 3},
			{"name", "Knight"},
			{"speed", 100},
			{"health", 100},
			{"dexterity", 100},
			{"intelligence", 100}
		},
		new Godot.Collections.Dictionary
		{
			{"character_id", 4},
			{"name", "Mage"},
			{"speed", 100},
			{"health", 100},
			{"dexterity", 100},
			{"intelligence", 100}
		}
	};
	
	
	public override void _Ready()
	{
		_labelHealth=GetNode<Label>("%Label_health");
		_labelSpeed=GetNode<Label>("%Label_speed");
		_labelDexterity=GetNode<Label>("%Label_dexterity");
		_labelIntelligence=GetNode<Label>("%Label_intelligence");
		
		_labelCharacterName=GetNode<Label>("%Label_SelectedCharacterName");
		_ButtonUpgradeUnlock=GetNode<Button>("%Button_UpgradeUnlock");
		_Button_Select=GetNode<Button>("%Button_Select");
		
		HttpRequest httpRequest = GetNode<HttpRequest>("%HTTPRequest");
		httpRequest.RequestCompleted += OnRequestCompleted;
		
		string jsonData = "{\"user_id\": \"D465D2FD-092C-40A6-945D-E29E99FA524A\"}";
		Error error = httpRequest.Request("http://localhost:3000/api/v1/protected/getAllCharacters", new string[] {"Content-Type: application/json"}, HttpClient.Method.Post, jsonData);
		if (error != Error.Ok)
  		{
	   		GD.Print("Request nicht gesendet");
		}
		
		
		
	}
	 private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
			string receivedString= Encoding.UTF8.GetString(body);
			GD.Print("Antwort empfangen: " + receivedString);
			Variant receivedVar = Json.ParseString(receivedString);
			allCharacters = receivedVar.AsGodotArray();
			
			//hier wir bis jetzt einfach immer beim aufrufen charcter 1 ausgewählt. 
			//Muss geändert werden, dass hier der beim letzten aufruf ausgewöhlte charcter zu beginn ausgewählt ist
			_currentlySelectedCharacter=GetNode<Button>("%Button_Character1");
			_on_button_select_pressed();
			SetCharacterPageValues(allCharacters,1);
	}
	
	// Text der charctersite wird an den Text welcher in der DB unter dem chracter mit der nummer (characterId) hinterlegt ist 
	private void SetCharacterPageValues(Godot.Collections.Array characters, int characterId)
	{
		foreach (Godot.Collections.Dictionary character in characters)
		{
			int character_id =(int)character["character_id"];
			if(characterId==character_id){
			string name =(string)character["name"];
			int speed =(int)character["speed"];
			int health =(int)character["health"];
			int dexterity =(int) character["dexterity"];
			int intelligence =(int) character["intelligence"];
			
			_labelCharacterName.Text=name;
			_labelHealth.Text=$"Health {health}";
			_labelSpeed.Text=$"Speed {speed}";
			_labelDexterity.Text=$"Dexterity {dexterity}";
			_labelIntelligence.Text=$"Intelligence {intelligence}";
			}
		}
		
	}
	
	
	private void _on_button_back_charactermenu_pressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/mainMenu.tscn");
		GetTree().ChangeSceneToPacked(scene);
	}
	
	//wenn ein character button angeklickt wird
	public void _characterSelected(ButtonsCharacterSelection button)
	{
		if(button!=null && _labelCharacterName!=null){
			SetCharacterPageValues(allCharacters,int.Parse(button.Text));
			_currentlySelectedCharacter=button;
			if(button.Name=="Button_Character6"||button.Name=="Button_Character3"){ //check if character is unlocken. locked=true
				_ButtonUpgradeUnlock.Text="Unlock";
				_Button_Select.Disabled=true;
			}else{
				_ButtonUpgradeUnlock.Text="Upgrade";
				_Button_Select.Disabled=false;
			}
		}
	}
	
	//wenn bei einem ausgewöhlten charcter der "select"- button gedrückt wird 
	private void _on_button_select_pressed( )
	{
		if(_currentlySelectedCharacter!=null)
		{
			
			//GD.Print(_oldSelectedCharacter+"  "+_currentlySelectedCharacter);
			var style = new StyleBoxFlat();
			style.BgColor = Color.Color8(0x4F,0x4F,0x4F);
			style.BorderColor= new Color(1f,0f,0f);
			style.SetBorderWidth(Side.Top,3);
			style.SetBorderWidth(Side.Bottom,3);
			style.SetBorderWidth(Side.Left,3);
			style.SetBorderWidth(Side.Right,3);
			_currentlySelectedCharacter.AddThemeStyleboxOverride("normal",style);
			
		if(_oldSelectedCharacter!=_currentlySelectedCharacter){
		_resetButton(_oldSelectedCharacter);
		_oldSelectedCharacter=_currentlySelectedCharacter;
		 }
		}
	}
	private void _resetButton(Button button){
		if(button!=null){
		var style = new StyleBoxFlat();
		style.BgColor = Color.Color8(0x4F,0x4F,0x4F);
		button.AddThemeStyleboxOverride("normal",style);
		}
	}
		
	
	
}
