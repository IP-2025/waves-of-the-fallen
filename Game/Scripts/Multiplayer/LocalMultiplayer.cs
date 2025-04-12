using Godot;
using System;

public partial class LocalMultiplayer : Control
{
	private int port = 9999; // multiplayer port
    private string address = "127.0.0.1"; // 127.0.0.1 = localhost
    private ENetMultiplayerPeer peer;

	// Called when the node enters the scene tree for the first time.
      public override void _Ready()
    {
        GD.Print("Instantiating local multiplayer");
		Multiplayer.PeerConnected += PeerConnected;
        Multiplayer.PeerDisconnected += PeerDisconnected;
        Multiplayer.ConnectedToServer += ConnectedToServer;
        Multiplayer.ConnectionFailed += ConnectionFailed;
    }

    // runs if connection fails, runs only on client
    private void ConnectionFailed()
    {
        GD.Print("Connection failed!");
    }

    // runs if connection is successful, runs only on client
    private void ConnectedToServer()
    {
        GD.Print("Connected to server");
        // Rpc to Server (has allways id 1) for new players information, server will then spread it to everyone
        RpcId(
            1,
            "SendPlayerInformation",
            Multiplayer.GetUniqueId().ToString(), // later fill in users custom name from a LineEdit
            Multiplayer.GetUniqueId()
        );
    }

    // runs if a player disconnects, runs on all peers
    // id = id of the player that disconnected
    private void PeerDisconnected(long id)
    {
        GD.Print("Player disconnected: " + id.ToString());
    }

    // runs if a player connects, runs on all peers
    private void PeerConnected(long id)
    {
        GD.Print("Player Connected: " + id.ToString());
    }

    public void Join()
    {
        peer = new ENetMultiplayerPeer();
        peer.CreateClient(address, port); // be a client to ip and port (= server = host)

        // compressing packages to bring down bandwidth MUST BE SAME AS HOST / SERVER to be able to decompress
        // RangeCoder good for smaller packages up to about 4 KB, larger packages = inefficient
        peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
        Multiplayer.MultiplayerPeer = peer;
        GD.Print("Joining game");
    }

    public void Host()
    {
        peer = new ENetMultiplayerPeer();
        var error = peer.CreateServer(port, 4); // 4 = max 4 players
        if (error != Error.Ok)
        {
            GD.Print("ERROR cannot host: " + error.ToString());
            return; // to stop server execution ...
        }

        peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder); //  MUST BE SAME AS CLIENT to be able to decompress

        // set self to multiplayer peer to actually connect to the server .. if not, server exists, but self is not connected (not peer)
        Multiplayer.MultiplayerPeer = peer;
        GD.Print("Waiting for players...");
        // Host has to send its player information to its self to be part of the game. No RPC needet because if host opens lobby, he is alone
        SendPlayerInformation(
            Multiplayer.GetUniqueId().ToString(), // later fill in users custom name from a LineEdit,
            1
        );
    }

    public void Play()
    {
        Rpc("StartGame");
       //StartGame();
    }

    // This (AnyPeer) makes sure that everyone (means all peers) recives / executes this.
    // callLocal = true to make sure tat it is called by self
    // Reliable = TCP connection wit ACK send back ... larger round trip time because its waiting for ACK but
    // this makes sure that this important data is safely transmitted.
    [Rpc(
        MultiplayerApi.RpcMode.Authority,
        CallLocal = true,
        TransferMode = MultiplayerPeer.TransferModeEnum.Reliable
    )]
    private void StartGame()
    {
        // loading and instantiating world scene
        var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Main.tscn").Instantiate<Node>();
	    // GetTree().ChangeSceneToPacked(scene);
        GetTree().Root.AddChild(scene); // add world as child
        this.Hide(); // hiding main menue..

        GD.Print("Game started");
        foreach (var item in GameManager.Players)
        {
            GD.Print(item.Name + " is playing");
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void SendPlayerInformation(string name, int id)
    {
        PlayerInfo playerInfo = new PlayerInfo() { Name = name, Id = id };
        if (!GameManager.Players.Contains(playerInfo))
        {
            GameManager.Players.Add(playerInfo); // if a new player joins, its information is added to GameManager
        }

        // spreading every players information to everyone
        if (Multiplayer.IsServer())
        {
            foreach (var item in GameManager.Players)
            {
                Rpc("SendPlayerInformation", item.Name, item.Id); // sending it
            }
        }
    }
}
