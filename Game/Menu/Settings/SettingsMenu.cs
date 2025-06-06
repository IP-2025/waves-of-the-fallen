using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

public partial class SettingsMenu : Control
{
	private SettingsManager settingsManager;
	private Button buttonLanguage;
	private int menuBackgroundMusicBusIndex;
	private int soundEffectBusIndex;

	private readonly List<string> languages = ["English", "German"];

	private Dictionary<string, Variant> audioSettings;
	private Dictionary<string, Variant> soundSettings;

	public override void _Ready()
	{
		settingsManager = GetNode<SettingsManager>("/root/SettingsManager");
		CheckBox musicEnabledCheckbox = GetNode<CheckBox>("%CheckBoxMusicEnabled");
		CheckBox soundEnabledCheckbox = GetNode<CheckBox>("%CheckBoxSoundEnabled");
		HSlider sliderMusic = GetNode<HSlider>("%SliderMusicVolume");
		HSlider sliderSound = GetNode<HSlider>("%SliderSoundVolume");
		buttonLanguage = GetNode<Button>("%ButtonLanguage");
		audioSettings = settingsManager.LoadSettingSection("Audio");
		soundSettings = settingsManager.LoadSettingSection("Sound");
		menuBackgroundMusicBusIndex = AudioServer.GetBusIndex("MenuBackgroundMusicBus");
		soundEffectBusIndex = AudioServer.GetBusIndex("SoundEffectBus");
		sliderMusic.Value = Mathf.DbToLinear(
			AudioServer.GetBusVolumeDb(menuBackgroundMusicBusIndex)
		);
		sliderSound.Value = Mathf.DbToLinear(
			AudioServer.GetBusVolumeDb(soundEffectBusIndex)
		);
		musicEnabledCheckbox.ButtonPressed = (bool)audioSettings["Enabled"];
		AudioServer.SetBusMute(menuBackgroundMusicBusIndex, !(bool)audioSettings["Enabled"]);
		soundEnabledCheckbox.ButtonPressed = (bool)soundSettings["Enabled"];
		AudioServer.SetBusMute(soundEffectBusIndex, !(bool)soundSettings["Enabled"]);
		buttonLanguage.Text = (string)settingsManager.LoadSettingSection("General")["Language"];
	}

	private void _on_button_back_settings_pressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Menu/Main/mainMenu.tscn");
		SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
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

	private void _on_h_slider_sound_volume_value_changed(float volumeSlider)
	{
		AudioServer.SetBusVolumeDb(soundEffectBusIndex, Mathf.LinearToDb(volumeSlider));
		settingsManager.SaveSetting("Sound", "Volume", volumeSlider);
	}

	private void _on_check_box_sound_enabled_toggled(bool toggled_on)
	{
		AudioServer.SetBusMute(soundEffectBusIndex, !toggled_on);
		settingsManager.SaveSetting("Sound", "Enabled", toggled_on);
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
		SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
	}
}
