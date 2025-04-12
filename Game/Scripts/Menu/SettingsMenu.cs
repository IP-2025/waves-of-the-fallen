using Godot;
using System;

public partial class SettingsMenu : Control
{
 private HSlider sliderMusic;
 private int menuBackgroundMusicBusIndex;

public override void _Ready()
{
	sliderMusic = GetNode<HSlider>("MarginContainer2/VBoxContainer/MarginContainer/VBoxContainer/HBoxContainer3/HSliderMusic");
	menuBackgroundMusicBusIndex=AudioServer.GetBusIndex("MenuBackgroundMusicBus");
	AudioServer.SetBusVolumeDb(menuBackgroundMusicBusIndex,Mathf.LinearToDb(0));
	//GD.Print(AudioServer.GetBusVolumeDb(menuBackgroundMusicBusIndex).ToString("0.000"));
	
	
}
  private void _on_button_back_settings_pressed()
  {
	var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/mainMenu.tscn");
	GetTree().ChangeSceneToPacked(scene);
  }
  private void _on_h_slider_music_value_changed(float volumeSlider)
  { 
	GD.Print(volumeSlider.ToString("0.00"));
	AudioServer.SetBusVolumeDb(menuBackgroundMusicBusIndex,Mathf.LinearToDb(volumeSlider));
	
  }
}
