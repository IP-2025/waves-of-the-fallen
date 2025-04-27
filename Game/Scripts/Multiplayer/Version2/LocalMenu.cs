using Godot;


public partial class LocalMenu : Control {
    private Button playButton;
    public override void _Ready() {
        playButton = GetNode<Button>("PlayButton");
        playButton.Connect("pressed", Callable.From(OnPlay));
    }
    private void OnPlay() {
        // Szene wechselt zu Game
        var gameScene = GD.Load<PackedScene>("res://Scenes/Game.tscn");
        GetTree().ChangeSceneToPacked(gameScene);
        // Network starten
        var nm = GetNode<NetworkManager>("/root/NetworkManager");
        if (IsHost) nm.InitServer(); else nm.InitClient(ipInput.Text);
    }
}