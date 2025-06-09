using System.Collections.Generic;
using Game.Utilities.Backend;
using Godot;

namespace Game.Menu.Settings;

public partial class SettingsMenu : Control
{
	private CharacterManager _characterManager;
	private SettingsManager _settingsManager;
	private Button _buttonLanguage;
	private int _menuBackgroundMusicBusIndex;
	private int _soundEffectBusIndex;

	private readonly List<string> _languages = ["English", "German"];

	private Dictionary<string, Variant> _soundSettings;
	private Dictionary<string, Variant> _audioSettings;

	private Button _deleteAccountButton;
	private ColorRect _deletePopup;
	private Label _deleteLabel;
	private Button _yesButton;
	private Button _cancelButton;
	private HttpRequest _deleteAccountRequest;

	public override void _Ready()
	{
		_settingsManager = GetNode<SettingsManager>("/root/SettingsManager");
		var soundEnabledCheckbox = GetNode<CheckBox>("%CheckBoxSoundEnabled");
		var sliderSound = GetNode<HSlider>("%SliderSoundVolume");
		_buttonLanguage = GetNode<Button>("%ButtonLanguage");
		_audioSettings = _settingsManager.LoadSettingSection("Audio");
		_soundSettings = _settingsManager.LoadSettingSection("Sound");
		_menuBackgroundMusicBusIndex = AudioServer.GetBusIndex("MenuBackgroundMusicBus");
		_soundEffectBusIndex = AudioServer.GetBusIndex("SoundEffectBus");
		_characterManager = GetNode<CharacterManager>("/root/CharacterManager");
		_settingsManager = GetNode<SettingsManager>("/root/SettingsManager");
		var musicEnabledCheckbox = GetNode<CheckBox>("%CheckBoxMusicEnabled");
		var sliderMusic = GetNode<HSlider>("%SliderMusicVolume");
		_buttonLanguage = GetNode<Button>("%ButtonLanguage");
		_audioSettings = _settingsManager.LoadSettingSection("Audio");
		_menuBackgroundMusicBusIndex = AudioServer.GetBusIndex("MenuBackgroundMusicBus");
		sliderMusic.Value = Mathf.DbToLinear(
			AudioServer.GetBusVolumeDb(_menuBackgroundMusicBusIndex)
		);
		sliderSound.Value = Mathf.DbToLinear(
			AudioServer.GetBusVolumeDb(_soundEffectBusIndex)
		);
		musicEnabledCheckbox.ButtonPressed = (bool)_audioSettings["Enabled"];
		AudioServer.SetBusMute(_menuBackgroundMusicBusIndex, !(bool)_audioSettings["Enabled"]);
		soundEnabledCheckbox.ButtonPressed = (bool)_soundSettings["Enabled"];
		AudioServer.SetBusMute(_soundEffectBusIndex, !(bool)_soundSettings["Enabled"]);
		_buttonLanguage.Text = (string)_settingsManager.LoadSettingSection("General")["Language"];
		
		_deleteAccountButton = GetNode<Button>("%ButtonDeleteAccount");
		_deletePopup = GetNode<ColorRect>("%ColorRect2");
		_deleteLabel = GetNode<Label>("%Label");
		_yesButton = GetNode<Button>("%YesButton");
		_cancelButton = GetNode<Button>("%CancleButton");
		_deleteAccountRequest = GetNode<HttpRequest>("%DeleteAccountRequest");

		_deleteAccountButton.Connect("pressed", new Callable(this, nameof(OnDeleteAccountButtonPressed)));
		_deletePopup.Visible = false;
		_yesButton.Connect("pressed", new Callable(this, nameof(OnYesButtonPressed)));
		_cancelButton.Connect("pressed", new Callable(this, nameof(OnCancelButtonPressed)));
		_deleteAccountRequest.Connect("request_completed", new Callable(this, nameof(OnDeleteAccountRequestCompleted)));
	}

	private void OnDeleteAccountButtonPressed()
	{
		_deletePopup.Visible = true;
		_deleteLabel.Text = GameState.CurrentState == ConnectionState.Offline ? "You are currently offline. Please connect to the internet to delete your account.\n If you want to delete your local save data press 'Yes'." : "Are you sure you want to delete your account? This action cannot be undone.";
		SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
	}

	private void OnYesButtonPressed()
	{
		if (GameState.CurrentState == ConnectionState.Online)
		{
			const string url = $"{ServerConfig.BaseUrl}/api/v1/protected/user";
			var token = SecureStorage.LoadToken();
			var headers = new[] { $"Authorization: Bearer {token}" };
			var err = _deleteAccountRequest.Request(url, headers, HttpClient.Method.Delete);
			if (err != Error.Ok)
				GD.PrintErr($"DeleteAccountRequest error: {err}");
		}

		SecureStorage.DeleteToken();
		if (FileAccess.FileExists("user://character_settings.cfg"))
		{
			_characterManager.SaveCharacterData(1, "Archer", 85, 200, 100, 110, 1, 1);
			_characterManager.SaveCharacterData(2, "Assassin", 70, 220, 100, 110, 1, 0);
			_characterManager.SaveCharacterData(3, "Knight", 125, 180, 125, 85, 1, 0);
			_characterManager.SaveCharacterData(4, "Mage", 100, 200, 110, 110, 1, 0);
			_characterManager.SetGold(0);
		}
		else
		{
			GD.Print("Datei nicht gefunden.");
		}

		if (GameState.CurrentState == ConnectionState.Offline)
		{
			var scene = ResourceLoader.Load<PackedScene>("res://Menu/Login/login_screen.tscn");
			SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
			GetTree().ChangeSceneToPacked(scene);
		}
	}

	private void OnCancelButtonPressed()
	{
		_deletePopup.Visible = false;
		SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
	}

	private void OnDeleteAccountRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		GD.Print("The response code is: " + responseCode);
		if (responseCode == 200)
		{
			GD.Print("Account deleted successfully.");
			var scene = ResourceLoader.Load<PackedScene>("res://Menu/Login/login_screen.tscn");
			SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
			GetTree().ChangeSceneToPacked(scene);
		}
		else
		{
			GD.PrintErr($"Failed to delete account. Response code: {responseCode}");
		}
	}

	private void _on_button_back_settings_pressed()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Menu/Main/mainMenu.tscn");
		SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
		GetTree().ChangeSceneToPacked(scene);
	}

	private void _on_h_slider_music_value_changed(float volumeSlider)
	{
		AudioServer.SetBusVolumeDb(_menuBackgroundMusicBusIndex, Mathf.LinearToDb(volumeSlider));
		_settingsManager.SaveSetting("Audio", "Volume", volumeSlider);
	}

	private void _on_check_box_music_enabled_toggled(bool toggledOn)
	{
		AudioServer.SetBusMute(_menuBackgroundMusicBusIndex, !toggledOn);
		_settingsManager.SaveSetting("Audio", "Enabled", toggledOn);
	}

	private void _on_h_slider_sound_volume_value_changed(float volumeSlider)
	{
		AudioServer.SetBusVolumeDb(_soundEffectBusIndex, Mathf.LinearToDb(volumeSlider));
		_settingsManager.SaveSetting("Sound", "Volume", volumeSlider);
	}

	private void _on_check_box_sound_enabled_toggled(bool toggledOn)
	{
		AudioServer.SetBusMute(_soundEffectBusIndex, !toggledOn);
		_settingsManager.SaveSetting("Sound", "Enabled", toggledOn);
	}

	private void _on_button_language_pressed()
	{
		var currentLanguageIndex = _languages.IndexOf(
			(string)_settingsManager.LoadSettingSection("General")["Language"]
		);
		var nextLanguageIndex = (currentLanguageIndex + 1) % _languages.Count;
		var nextLanguage = _languages[nextLanguageIndex];
		_settingsManager.SaveSetting("General", "Language", nextLanguage);
		_buttonLanguage.Text = nextLanguage;
		SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
	}
	
	public override void _Notification(int what)
	{
		if (what == NotificationWMGoBackRequest)
		{
			_on_button_back_settings_pressed();
		}   
	}
}