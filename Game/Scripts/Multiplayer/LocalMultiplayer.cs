using System;
using System.Linq;
using Godot;
using System.Diagnostics;
public partial class LocalMultiplayer : Control
{
    private int port = 9999; // multiplayer port
    private string address = "127.0.0.1"; // 127.0.0.1 = localhost
    private ENetMultiplayerPeer peer;
    public bool enableDebug = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        DebugIt("Instantiating local multiplayer");
        Multiplayer.PeerConnected += PeerConnected;
        Multiplayer.PeerDisconnected += PeerDisconnected;
        Multiplayer.ConnectedToServer += ConnectedToServer;
        Multiplayer.ConnectionFailed += ConnectionFailed;
        Name = Multiplayer.GetUniqueId().ToString();
    }

    // runs if connection fails, runs only on client
    private void ConnectionFailed()
    {
        DebugIt("Connection failed!");
    }

    // runs if connection is successful, runs only on client
    private void ConnectedToServer()
    {
        DebugIt("Connected to server");
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
        DebugIt("Player disconnected: " + id.ToString());
    }

    // runs if a player connects, runs on all peers
    private void PeerConnected(long id)
    {
        DebugIt("Player Connected: " + id.ToString());
    }

    public void Join()
    {
        peer = new ENetMultiplayerPeer();
        peer.CreateClient(address, port); // be a client to ip and port (= server = host)

        // compressing packages to bring down bandwidth MUST BE SAME AS HOST / SERVER to be able to decompress
        // RangeCoder good for smaller packages up to about 4 KB, larger packages = inefficient
        peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
        Multiplayer.MultiplayerPeer = peer;

        DebugIt((GameManager.Players.Count, " Players connected").ToString());
    }

    public void Host()
    {
        peer = new ENetMultiplayerPeer();
        var error = peer.CreateServer(port);
        if (error != Error.Ok)
        {
            GD.PrintErr("ERROR cannot host: " + error.ToString());
            return; // to stop server execution ...
        }

        peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder); //  MUST BE SAME AS CLIENT to be able to decompress

        // set self to multiplayer peer to actually connect to the server .. if not, server exists, but self is not connected (not peer)
        Multiplayer.MultiplayerPeer = peer;
        DebugIt("Waiting for players...");
        // Host has to send its player information to its self to be part of the game. No RPC needet because if host opens lobby, he is alone
        SendPlayerInformation(
            Multiplayer.GetUniqueId().ToString(), // later fill in users custom name from a LineEdit,
            1
        );
    }

    public void Play()
    {
        foreach (var item in GameManager.Players)
        {
            // spreading active players information from host to everyone
            if (Multiplayer.IsServer())
            {
                DebugIt(("Server -> Client: Transmitting player: Name=", item.Name.ToString(), " Id=", item.Id.ToString()).ToString());
                Rpc("SendPlayerInformation", item.Name, item.Id); // sending it
            }
        }

        Rpc("StartGame");
    }

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

        DebugIt("Game started");
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false)]
    private void SendPlayerInformation(string name, int id)
    {
        DebugIt("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
        DebugIt((Multiplayer.GetUniqueId().ToString(), " prints this:").ToString());

        PlayerInfo playerInfo = new PlayerInfo() { Name = name, Id = id };
        if (!GameManager.Players.Any(player => player.Id == playerInfo.Id))
        {
            GameManager.Players.Add(playerInfo); // if a new player joins, its information is added to host s GameManager
            DebugIt((Multiplayer.GetUniqueId().ToString(), " added -> Name: ", name, " id: ", id).ToString());
        }

        DebugIt(("Totale of ", GameManager.Players.Count, " players connected").ToString());
        DebugIt("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
    }

    private void DebugIt(string message)
    {
        if (enableDebug)
        {
            Debug.Print(message);
        }
    }
}
