using System;
using System.Collections.Generic;
using Godot;

public partial class SettingsMenu : Control
{
	private SettingsManager settingsManager;
	private Button buttonLanguage;
	private int menuBackgroundMusicBusIndex;

	private readonly List<string> languages = ["English", "German"];

	private Dictionary<string, Variant> audioSettings;

	public override void _Ready()
	{
		settingsManager = GetNode<SettingsManager>("/root/SettingsManager");
		CheckBox musicEnabledCheckbox = GetNode<CheckBox>("%CheckBoxMusicEnabled");
		HSlider sliderMusic = GetNode<HSlider>("%SliderMusicVolume");
		buttonLanguage = GetNode<Button>("%ButtonLanguage");
		audioSettings = settingsManager.LoadSettingSection("Audio");
		menuBackgroundMusicBusIndex = AudioServer.GetBusIndex("MenuBackgroundMusicBus");
		sliderMusic.Value = Mathf.DbToLinear(
			AudioServer.GetBusVolumeDb(menuBackgroundMusicBusIndex)
		);
		musicEnabledCheckbox.ButtonPressed = (bool)audioSettings["Enabled"];
		AudioServer.SetBusMute(menuBackgroundMusicBusIndex, !(bool)audioSettings["Enabled"]);
		buttonLanguage.Text = (string)settingsManager.LoadSettingSection("General")["Language"];
	}

	private void _on_button_back_settings_pressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/mainMenu.tscn");
		SoundManager.Instance.PlayUI();
		GetTree().ChangeSceneToPacked(scene);
	}

	private void _on_h_slider_music_value_changed(float volumeSlider)
	{
		AudioServer.SetBusVolumeDb(menuBackgroundMusicBusIndex, Mathf.LinearToDb(volumeSlider));
		settingsManager.SaveSetting("Audio", "Volume", volumeSlider);
	}

	private void _on_check_box_music_enabled_toggled(bool toggled_on)
	{
		AudioServer.SetBusMute(menuBackgroundMusicBusIndex, !toggled_on);
		settingsManager.SaveSetting("Audio", "Enabled", toggled_on);
	}

	private void _on_button_language_pressed()
	{
		int currentLanguageIndex = languages.IndexOf(
			(string)settingsManager.LoadSettingSection("General")["Language"]
		);
		int nextLanguageIndex = (currentLanguageIndex + 1) % languages.Count;
		string nextLanguage = languages[nextLanguageIndex];
		settingsManager.SaveSetting("General", "Language", nextLanguage);
		buttonLanguage.Text = nextLanguage;
		SoundManager.Instance.PlayUI();
	}
}
