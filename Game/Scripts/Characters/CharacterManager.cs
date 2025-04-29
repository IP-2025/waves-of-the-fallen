using Godot;

public partial class CharacterManager : Node
{
	private ConfigFile config = new();
	private const string SettingsPath = "user://character_settings.cfg";
	private const string Section = "Character";
	private const string Key = "LastSelectedID";

	public override void _Ready()
	{
		if (FileAccess.FileExists(SettingsPath))
		{
			config.Load(SettingsPath);
		}
		else
		{
			// Standardwert setzen
			config.SetValue(Section, Key, 1);
			config.Save(SettingsPath);
		}
	}

	public void SaveLastSelectedCharacterID(int characterId)
	{
		config.SetValue(Section, Key, characterId);
		config.Save(SettingsPath);
	}

	public int LoadLastSelectedCharacterID()
	{
		return (int)config.GetValue(Section, Key, 1); // -1 falls nicht gesetzt
	}
}
