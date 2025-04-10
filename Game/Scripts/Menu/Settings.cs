using Godot;
using System;

public partial class Settings : Control
{
  private void _on_button_back_settings_pressed()
  {
    var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/mainmenu.tscn");
    GetTree().ChangeSceneToPacked(scene);
  }
}
