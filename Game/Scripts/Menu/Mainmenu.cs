using Godot;
using System;

public partial class Mainmenu : Control
{
  private void _on_button_charactermenu_pressed()
  {
    var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/character.tscn");
    GetTree().ChangeSceneToPacked(scene);
  }
  private void _on_button_settings_pressed()
  {
    var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/settings.tscn");
    GetTree().ChangeSceneToPacked(scene);
  }
  private void _on_button_play_pressed()
  {
    var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/online_local.tscn");
    GetTree().ChangeSceneToPacked(scene);
  }
}
