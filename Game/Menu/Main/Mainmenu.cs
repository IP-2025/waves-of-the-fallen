using Godot;
using System;

public partial class Mainmenu : Control
{
  private void _on_button_charactermenu_pressed()
  {
	var scene = ResourceLoader.Load<PackedScene>("res://Menu/Character/characterMenu.tscn");
	GetTree().ChangeSceneToPacked(scene);
  }
  private void _on_button_settings_pressed()
  {
	var scene = ResourceLoader.Load<PackedScene>("res://Menu/Settings/settingsMenu.tscn");
  SoundManager.Instance.PlayUI();
	GetTree().ChangeSceneToPacked(scene);
  }
  private void _on_button_play_pressed()
  {
	var scene = ResourceLoader.Load<PackedScene>("res://Menu/online_localMenu.tscn");
  SoundManager.Instance.PlayUI();
	GetTree().ChangeSceneToPacked(scene);
  }

  private void _on_button_highscore_pressed()
  {
	  var scene = ResourceLoader.Load<PackedScene>("res://Menu/HighscoreList/highscore_screen.tscn");
	  SoundManager.Instance.PlayUI();
	  GetTree().ChangeSceneToPacked(scene);
  }
}
