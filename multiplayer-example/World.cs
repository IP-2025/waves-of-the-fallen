using System;
using System.ComponentModel;
using Godot;

public partial class World : Node
{
    private PanelContainer mainMenu;
    private Button joinButton;
    private LineEdit addressEntry;
    private Button hostButton;

    private readonly PackedScene playerScene = (PackedScene)ResourceLoader.Load("res://player.tscn");
    const int PORT = 9999;
    private ENetMultiplayerPeer enetPeer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Engine.MaxFps = 60;
        mainMenu = GetNode<PanelContainer>("CanvasLayer/MainMenue");
        joinButton = GetNode<Button>("CanvasLayer/MainMenue/MarginContainer/VBoxContainer/JoinButton");
        addressEntry = GetNode<LineEdit>("CanvasLayer/MainMenue/MarginContainer/VBoxContainer/AddressEntry");
        hostButton = GetNode<Button>("CanvasLayer/MainMenue/MarginContainer/VBoxContainer/HostButton");


        enetPeer = new ENetMultiplayerPeer();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    private void _on_join_button_pressed()
    {
        mainMenu.Hide();

        enetPeer.CreateClient("localhost", PORT);
        Multiplayer.MultiplayerPeer = enetPeer;

        // WICHTIG: eigenen Spieler direkt anlegen, sobald Client verbunden ist
        Multiplayer.ConnectedToServer += () =>
        {
            _addPlayer(Multiplayer.GetUniqueId());
        };
    }


    private void _on_host_button_pressed()
    {
        mainMenu.Hide();

        enetPeer.CreateServer(PORT);
        Multiplayer.MultiplayerPeer = enetPeer;
        Multiplayer.PeerConnected += _addPlayer;

        _addPlayer(Multiplayer.GetUniqueId());

    }

    private void _addPlayer(long peer_id)
    {
        Player player = (Player)playerScene.Instantiate();
        player.Name = peer_id.ToString();
        AddChild(player);
    }
}

