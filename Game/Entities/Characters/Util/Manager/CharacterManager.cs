using System;
using Godot;

public partial class CharacterManager : Node
{
    private ConfigFile _config = new();
    private const string SettingsPath = "user://character_settings.cfg";
    private const string Section = "Character";
    private const string Key = "LastSelectedID";
    private const string GoldKey = "gold";

    public override void _Ready()
    {
        GD.Print(ProjectSettings.GlobalizePath(SettingsPath)); //remove

        if (FileAccess.FileExists(SettingsPath))
        {
            _config.Load(SettingsPath);
        }
        else
        {
            // Standardwert setzen
            _config.SetValue(Section, Key, 1);

            //Standart Character und Stats setzen
            SaveCharacterData(1, "Archer", 100, 100, 100, 100, 1, 1);
            SaveCharacterData(2, "Assassin", 100, 100, 100, 100, 1, 1);
            SaveCharacterData(3, "Knight", 100, 100, 100, 100, 1, 1);
            SaveCharacterData(4, "Mage", 100, 100, 100, 100, 1, 0);

            _config.Save(SettingsPath);
        }

        // Gold initialisieren, falls noch nicht vorhanden
        if (_config.HasSectionKey(Section, GoldKey)) return;
        _config.SetValue(Section, GoldKey, 0);
        _config.Save(SettingsPath);
    }

    public void AddGold(int amount)
    {
        var currentGold = (int)_config.GetValue(Section, GoldKey, 0);
        GD.Print("Current Gold: " +currentGold);
        currentGold += amount;
        GD.Print("New Gold: " +currentGold);
        _config.SetValue(Section, GoldKey, currentGold);
        _config.Save(SettingsPath);
    }

    public void SubGold(int amount)
    {
        var currentGold = (int)_config.GetValue(Section, GoldKey, 0);
        currentGold = Math.Max(0, currentGold - amount);
        _config.SetValue(Section, GoldKey, currentGold);
        _config.Save(SettingsPath);
    }

    public void UpgradeCharacter(string characterId)
    {
        _config.SetValue(Section, Key, characterId);

        _config.SetValue(characterId, "health", LoadHealthByID(characterId) + 5);
        _config.SetValue(characterId, "speed", LoadSpeedByID(characterId) + 5);
        _config.SetValue(characterId, "dexterity", LoadDexterityByID(characterId) + 5);
        _config.SetValue(characterId, "intelligence", LoadIntelligenceByID(characterId) + 5);
        _config.SetValue(characterId, "level", LoadLevelByID(characterId) + 1);

        _config.Save(SettingsPath);
    }

    public void UpdateCharacter(string charcterId, int level, int unlocked)
    {
        _config.SetValue(charcterId, "level", level);
        _config.SetValue(charcterId, "unlocked", unlocked);
        _config.Save(SettingsPath);
    }

    public void SaveCharacterData(int characterId, string name, int health, int speed, int dexterity, int intelligence, int level, int unlocked)
    {
        _config.SetValue($"{characterId}", "name", name);
        _config.SetValue($"{characterId}", "health", health);
        _config.SetValue($"{characterId}", "speed", speed);
        _config.SetValue($"{characterId}", "dexterity", dexterity);
        _config.SetValue($"{characterId}", "intelligence", intelligence);
        _config.SetValue($"{characterId}", "level", level);
        _config.SetValue($"{characterId}", "unlocked", unlocked);
        _config.Save(SettingsPath);
    }

    public void SaveLastSelectedCharacterID(int characterId)
    {
        _config.SetValue(Section, Key, characterId);
        _config.Save(SettingsPath);
    }

    public string LoadNameByID(string id)
    {
        return (string)_config.GetValue(id, "name");
    }

    public int LoadHealthByID(string id)
    {
        return (int)_config.GetValue(id, "health");
    }

    public int LoadSpeedByID(string id)
    {
        return (int)_config.GetValue(id, "speed");
    }

    public int LoadDexterityByID(string id)
    {
        return (int)_config.GetValue(id, "dexterity");
    }

    public int LoadIntelligenceByID(string id)
    {
        return (int)_config.GetValue(id, "intelligence");
    }

    public int LoadLevelByID(string id)
    {
        return (int)_config.GetValue(id, "level");
    }

    public bool LoadIsUnlocked(string id)
    {
        return ((int)_config.GetValue(id, "unlocked") == 1);
    }

    public void SetUnlocked(string id)
    {
        _config.SetValue(id, "unlocked", 1);
        _config.Save(SettingsPath);
    }

    public int LoadLastSelectedCharacterID()
    {
        return (int)_config.GetValue(Section, Key, 1);
    }

    public void SaveSelectedCharacter(string characterName)
    {
        _config.SetValue(Section, "SelectedCharacter", characterName);
        _config.Save(SettingsPath);
        GD.Print($"Charakter gespeichert: {characterName}");
    }

    public string LoadSelectedCharacter()
    {
        return (string)_config.GetValue(Section, "SelectedCharacter", "DefaultPlayer");
    }

    public int LoadGold()
    {
        return (int)_config.GetValue(Section, GoldKey, 0);
    }
}