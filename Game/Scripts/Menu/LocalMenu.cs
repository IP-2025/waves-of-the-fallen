using Godot;
using System;

public partial class LocalMenu : Control
{
  private LocalMultiplayer LocalMultiplayer;  

  public override void _Ready()
    {
      LocalMultiplayer = new LocalMultiplayer();
      AddChild(LocalMultiplayer); // fügt LocalMultiplayer als Child hinzu

    }
  private void _on_button_back_local_pressed()
  {
	var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/online_localMenu.tscn");
	GetTree().ChangeSceneToPacked(scene);
  }
  private void _on_join_button_pressed()
  {
    LocalMultiplayer.Join();
  }
  private void _on_host_button_pressed()
  {
    LocalMultiplayer.Host();
  }
   private void _on_play_button_pressed()
  {
    LocalMultiplayer.Play();
  }
}
