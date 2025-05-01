using Godot;
using System;

public partial class OnlineLocalMenu : Control
{
  private void _on_button_back_onlineLocal_pressed()
  {
    var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/mainMenu.tscn");
    GetTree().ChangeSceneToPacked(scene);
  }
  private void _on_button_local_pressed()
  {
    var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/localMenu.tscn");
    GetTree().ChangeSceneToPacked(scene);
  }
  private void _on_button_online_pressed()
  {

  }

  private void _on_button_solo_pressed()
  {
    NetworkManager.Instance.InitServer();

    var gameScene = GD.Load<PackedScene>("res://Scenes/GameRoot/GameRoot.tscn");
    gameScene.Instantiate<Node>();
    GetTree().ChangeSceneToPacked(gameScene);
  }
}
