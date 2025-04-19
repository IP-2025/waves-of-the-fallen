using Godot;
using System;

public partial class Charactermenu : Control
{
	private Label _labelCharacterName;
	private Button _currentlySelectedCharacter;
	private Button _oldSelectedCharacter;
	private Button _ButtonUpgradeUnlock;
	private Button _Button_Select;
	
	public override void _Ready()
	{
		_labelCharacterName=GetNode<Label>("%Label_SelectedCharacterName");
		_ButtonUpgradeUnlock=GetNode<Button>("%Button_UpgradeUnlock");
		_Button_Select=GetNode<Button>("%Button_Select");
	}
	private void _on_button_back_charactermenu_pressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/mainMenu.tscn");
		GetTree().ChangeSceneToPacked(scene);
	}
	
	public void _characterSelected(ButtonsCharacterSelection button)
	{
		if(button!=null && _labelCharacterName!=null){
			_labelCharacterName.Text=button.Name;
			_oldSelectedCharacter=_currentlySelectedCharacter;
			_currentlySelectedCharacter=button;
			if(1==1){ //check if character is unlocken. locked=true
				_ButtonUpgradeUnlock.Text="Unlock";
				_Button_Select.Disabled=true;
			}else{
				_ButtonUpgradeUnlock.Text="Upgrade";
			}
		}
	}
	
	private void _on_button_select_pressed( )
	{
		if(_currentlySelectedCharacter!=null)
		{
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
