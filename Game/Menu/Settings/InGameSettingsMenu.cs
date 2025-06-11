using Godot;

public partial class InGameSettingsMenu : CanvasLayer
{
	private int _musicBus;
	private int _soundBus;

	public override void _Ready()
	{
		_musicBus = AudioServer.GetBusIndex("MenuBackgroundMusicBus");
		_soundBus = AudioServer.GetBusIndex("SoundEffectBus");

		var musicCheck = GetNode<CheckBox>("%CheckBoxMusicEnabled");
		var musicSlider = GetNode<HSlider>("%SliderMusicVolume");
		var soundCheck = GetNode<CheckBox>("%CheckBoxSoundEnabled");
		var soundSlider = GetNode<HSlider>("%SliderSoundVolume");

		musicCheck.ButtonPressed = !AudioServer.IsBusMute(_musicBus);
		musicSlider.Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(_musicBus));
		soundCheck.ButtonPressed = !AudioServer.IsBusMute(_soundBus);
		soundSlider.Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(_soundBus));

		musicCheck.Toggled += toggled => AudioServer.SetBusMute(_musicBus, !toggled);
		musicSlider.ValueChanged += value => AudioServer.SetBusVolumeDb(_musicBus, Mathf.LinearToDb((float)value));
		soundCheck.Toggled += toggled => AudioServer.SetBusMute(_soundBus, !toggled);
		soundSlider.ValueChanged += value => AudioServer.SetBusVolumeDb(_soundBus, Mathf.LinearToDb((float)value));

		GetNode<Button>("%ButtonBack").Pressed += () => QueueFree();
	}
}
