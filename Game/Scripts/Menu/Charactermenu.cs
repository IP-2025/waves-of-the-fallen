using Godot;
using System;

public partial class Charactermenu : Control
{
	private Label _labelCharacterName;
	
	public override void _Ready()
	{
		_labelCharacterName=GetNode<Label>("%Label_SelectedCharacterName");
	}
	private void _on_button_back_charactermenu_pressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/mainMenu.tscn");
		GetTree().ChangeSceneToPacked(scene);
	}
	public void _characterSelected(ButtonsCharacterSelection button)
	{
		if(button!=null&&_labelCharacterName!=null){
		GD.Print(button.Name);
		_labelCharacterName.Text=button.Name;
		}
		
	}
}
