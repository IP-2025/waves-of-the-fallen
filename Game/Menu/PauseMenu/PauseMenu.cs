using Godot;
using Game.Utilities.Multiplayer;

public partial class PauseMenu : Control
{
    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always; // important to continue while paused
        GetNode<Button>("Background/VBoxContainer/Resume").Pressed += OnResumePressed;
        GetNode<Button>("Background/VBoxContainer/Settings").Pressed += OnSettingsPressed;
        GetNode<Button>("Background/VBoxContainer/Main Menu").Pressed += OnMainMenuPressed;
        GetNode<Button>("Background/VBoxContainer/Quit").Pressed += OnQuitPressed;
        Visible = false;
    }

    private void OnResumePressed()
    {
        Visible = false;
    
        if (NetworkManager.Instance._soloMode)
            GetTree().Paused = false;
    }

    private void OnSettingsPressed()
    {
        var settingsScene = GD.Load<PackedScene>("res://Menu/Settings/settingsMenu.tscn");
        var settingsMenu = settingsScene.Instantiate();
        GetTree().Root.AddChild(settingsMenu);
        Visible = false;
    }

    private void OnMainMenuPressed()
    {
        GetTree().ChangeSceneToFile("res://Menu/Main/mainMenu.tscn");
    }

    private void OnQuitPressed()
    {
        GetTree().Quit();
    }
}