using Godot;

public partial class PauseMenu : Control
{
    public override void _Ready()
    {
        GetNode<Button>("Background/VBoxContainer/Resume").Pressed += OnResumePressed;
        GetNode<Button>("Background/VBoxContainer/Settings").Pressed += OnSettingsPressed;
        GetNode<Button>("Background/VBoxContainer/Main Menu").Pressed += OnMainMenuPressed;
        GetNode<Button>("Background/VBoxContainer/Quit").Pressed += OnQuitPressed;
        Visible = true;
    }

    private void OnResumePressed() => Visible = false;

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