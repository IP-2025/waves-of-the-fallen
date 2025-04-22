using Godot;
using System;
using System.Collections.Generic;

public partial class SettingsMenu : Control
{
  private HSlider sliderMusic;
  private int menuBackgroundMusicBusIndex;
  private SettingsManager settingsManager;

  private Dictionary<string, Variant> audioSettings;

  public override void _Ready()
  {
	settingsManager = GetNode<SettingsManager>("/root/SettingsManager");
	audioSettings = settingsManager.LoadSettingSection("Audio");
	sliderMusic = GetNode<HSlider>("%SliderMusicVolume");
	menuBackgroundMusicBusIndex = AudioServer.GetBusIndex("MenuBackgroundMusicBus");
	sliderMusic.Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(menuBackgroundMusicBusIndex));
	AudioServer.SetBusMute(menuBackgroundMusicBusIndex, (bool)audioSettings["Enabled"]);
  }
  private void _on_button_back_settings_pressed()
  {
	var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/mainMenu.tscn");
	GetTree().ChangeSceneToPacked(scene);
  }
  private void _on_h_slider_music_value_changed(float volumeSlider)
  {
	AudioServer.SetBusVolumeDb(menuBackgroundMusicBusIndex, Mathf.LinearToDb(volumeSlider));

  }

  private void _on_check_box_music_enabled_toggled(bool toggled_on)
  {
	AudioServer.SetBusMute(menuBackgroundMusicBusIndex, !toggled_on);
	settingsManager.SaveSetting("Audio", "Enabled", toggled_on);
  }
}
