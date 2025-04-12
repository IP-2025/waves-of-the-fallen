using Godot;
using System;

public partial class LocalMenu : Control
{
  private void _on_button_back_local_pressed()
  {
	var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/online_localMenu.tscn");
	GetTree().ChangeSceneToPacked(scene);
  }
  private void _on_join_button_pressed()
  {
	  var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Main.tscn");
	  GetTree().ChangeSceneToPacked(scene);
  }
  private void _on_host_button_pressed()
  {
	//implement method
  }
}
