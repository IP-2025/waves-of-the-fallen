using Godot;
using System;

public partial class SettingsMenu : Control
{
  private void _on_button_back_settings_pressed()
  {
	var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/mainMenu.tscn");
	GetTree().ChangeSceneToPacked(scene);
  }
}
