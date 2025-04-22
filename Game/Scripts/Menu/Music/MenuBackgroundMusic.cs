using Godot;

public partial class MenuBackgroundMusic : AudioStreamPlayer
{
	private int menuBackgroundMusicBusIndex;

	public override void _Ready()
	{
		SettingsManager settingsManager = GetNode<SettingsManager>("/root/SettingsManager");
		menuBackgroundMusicBusIndex = AudioServer.GetBusIndex("MenuBackgroundMusicBus");
		bool musicEnabled = (bool)settingsManager.LoadSettingSection("Audio")["Enabled"];
		float musicVolume = (float)settingsManager.LoadSettingSection("Audio")["Volume"];
		AudioServer.SetBusVolumeDb(menuBackgroundMusicBusIndex, Mathf.LinearToDb(musicVolume));
		VolumeDb = Mathf.LinearToDb(musicVolume);
		AudioServer.SetBusMute(menuBackgroundMusicBusIndex, !musicEnabled);
		Play();
	}
}
