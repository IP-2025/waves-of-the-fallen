using Godot;
using System;

public partial class Mainmenu : Control
{
  private void _on_button_charactermenu_pressed()
  {
  var scene = ResourceLoader.Load<PackedScene>("res://Scenes/charactermenu.tscn");
  GetTree().ChangeSceneToPacked(scene);
  }

  private void _on_button_settings_pressed()
  {
  var scene = ResourceLoader.Load<PackedScene>("res://Scenes/settings.tscn");
  GetTree().ChangeSceneToPacked(scene);
  }
}
