using Godot;

public partial class CharacterManager : Node
{
	private ConfigFile config = new();
	private const string SettingsPath = "user://character_settings.cfg";
	private const string Section = "Character";
	private const string Key = "LastSelectedID";

	public override void _Ready()
	{
		GD.Print(ProjectSettings.GlobalizePath(SettingsPath)); //remove
		
		if (FileAccess.FileExists(SettingsPath))
		{
			config.Load(SettingsPath);
		}
		else
		{
			// Standardwert setzen
			config.SetValue(Section, Key, 1);
			
			//Standart Character und Stats setzen
			SaveCharacterData(1, "Archer", 100, 100, 100, 100, 1, 1);
			SaveCharacterData(2, "Assassin", 100, 100, 100, 100, 1, 1);
			SaveCharacterData(3, "Knight", 100, 100, 100, 100, 1, 1);
			SaveCharacterData(4, "Mage", 100, 100, 100, 100, 1, 0);
			
			config.Save(SettingsPath);
		}
	}
	
	public void UpgradeCharacter(string characterId)
	{
		config.SetValue(Section, Key, characterId);
		
		config.SetValue(characterId, "health", LoadHealthByID(characterId)+5);
		config.SetValue(characterId, "speed", LoadSpeedByID(characterId)+5);
		config.SetValue(characterId, "dexterity", LoadDexterityByID(characterId)+5);
		config.SetValue(characterId, "intelligence", LoadIntelligenceByID(characterId)+5);
		config.SetValue(characterId, "level", LoadLevelByID(characterId)+1);
		
		config.Save(SettingsPath);
	}
	
	public void SaveCharacterData(int characterId, string name, int health, int speed, int dexterity, int intelligence, int level, int unlocked)
	{
		config.SetValue($"{characterId}", "name", name);
		config.SetValue($"{characterId}", "health", health);
		config.SetValue($"{characterId}", "speed", speed);
		config.SetValue($"{characterId}", "dexterity", dexterity);
		config.SetValue($"{characterId}", "intelligence", intelligence);
		config.SetValue($"{characterId}", "level", level);
		config.SetValue($"{characterId}", "unlocked", unlocked);
		
		config.Save(SettingsPath);
	}

	public void SaveLastSelectedCharacterID(int characterId)
	{
		config.SetValue(Section, Key, characterId);
		config.Save(SettingsPath);
	}
	
	public string LoadNameByID(string id)
	{
		return (string)config.GetValue(id, "name");
	}
	
	public int LoadHealthByID(string id)
	{
		return (int)config.GetValue(id, "health");
	}
	
	public int LoadSpeedByID(string id)
	{
		return (int)config.GetValue(id, "speed");
	}
	
	public int LoadDexterityByID(string id)
	{
		return (int)config.GetValue(id, "dexterity");
	}
	
	public int LoadIntelligenceByID(string id)
	{
		return (int)config.GetValue(id, "intelligence");
	}
	public int LoadLevelByID(string id)
	{
		return (int)config.GetValue(id, "level");
	}
	
	public bool LoadIsUnlocked(string id)
	{
		return ((int)config.GetValue(id, "unlocked") == 1);
	}
	public void SetUnlocked(string id)
	{
		config.SetValue(id, "unlocked", 1);
		config.Save(SettingsPath);
	}

	public int LoadLastSelectedCharacterID()
	{
		return (int)config.GetValue(Section, Key, 1);
	}
}
