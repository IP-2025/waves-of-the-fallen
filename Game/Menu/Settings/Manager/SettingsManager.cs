using System.Collections.Generic;
using Godot;

public partial class SettingsManager : Node
{
	ConfigFile config = new();
	private const string SettingsPath = "user://settings.cfg";

	public override void _Ready()
	{
		if (!FileAccess.FileExists(SettingsPath))
		{
			config.SetValue("General", "Language", "English");
			config.SetValue("Audio", "Enabled", true);
			config.SetValue("Audio", "Volume", 0.1f);
			config.SetValue("Sound", "Enabled", true);
			config.SetValue("Sound", "Volume", 0.1f);
			config.Save(SettingsPath);
		}
		else
		{
			config.Load(SettingsPath);
		}
	}

	public Dictionary<string, Variant> LoadSettingSection(string section)
	{
		Dictionary<string, Variant> settings = [];
		string[] keys = config.GetSectionKeys(section);
		foreach (var key in keys)
		{
			var value = config.GetValue(section, key);
			settings.Add(key, value);
		}
		return settings;
	}

	public void SaveSetting(string section, string key, Variant value)
	{
		config.SetValue(section, key, value);
		config.Save(SettingsPath);
	}
}
