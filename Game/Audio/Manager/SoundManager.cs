using Godot;
using System;

public partial class SoundManager : Node2D
{
	public static SoundManager Instance;
	private int soundEffectBusIndex;

	public override void _Ready()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		SettingsManager settingsManager = GetNode<SettingsManager>("/root/SettingsManager");
		soundEffectBusIndex = AudioServer.GetBusIndex("SoundEffectBus");
		bool soundEnabled = (bool)settingsManager.LoadSettingSection("Sound")["Enabled"];
		float soundVolume = (float)settingsManager.LoadSettingSection("Sound")["Volume"];
		AudioServer.SetBusVolumeDb(soundEffectBusIndex, Mathf.LinearToDb(soundVolume));
		AudioServer.SetBusMute(soundEffectBusIndex, !soundEnabled);
	}

	public void PlaySoundAtPosition(AudioStreamPlayer2D player, Vector2 position, float volumeDb = 0.0f)
	{
		player.Position = position;
		player.VolumeDb = volumeDb;

		player.Play();
	}

	public void PlaySound(AudioStreamPlayer player, float volumeDb = 0.0f)
	{
		player.VolumeDb = volumeDb;
		player.Play();
	}
}
