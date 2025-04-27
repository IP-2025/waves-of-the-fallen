using System;
using System.Linq;
using Godot;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Runtime.ConstrainedExecution;

// Tis is for online multiplayer. The server simulates the multiplayer setup the host player dose in a local game
public partial class ServerBootstrapping : Node
{
    private int port;
    private string address;
    private readonly int MAX_PLAYERS = 4;
    private ENetMultiplayerPeer peer;
    public bool enableDebug = true;

    public override void _Ready()
    {
    }

    public void Init(int port)
    {
        this.port = port;

        address = GetServerIPAddress();
        DebugIt("Selected Active IP Address: " + address);

        // Create a new ENetMultiplayerPeer instance and set it as the multiplayer peer
        // This is the server side, so we create a server peer
        var peer = new ENetMultiplayerPeer();
        peer.CreateServer(port, MAX_PLAYERS);
        Multiplayer.MultiplayerPeer = peer;

        DebugIt("Instantiating local multiplayer");
        Multiplayer.PeerConnected += PeerConnected;
        Multiplayer.PeerDisconnected += PeerDisconnected;
        MultiplayerSynchronizer multiplayerSynchronizer = new MultiplayerSynchronizer();
    }


    // runs if a player disconnects, runs on all peers
    // id = id of the player that disconnected
    private void PeerDisconnected(long id)
    {
        DebugIt("Player disconnected: " + id.ToString());
        DeletePlayer(id); // remove player from GameManager
        Rpc("DeletePlayer", id);
    }


    private void PeerConnected(long id)  // runs if a player connects, runs on all peers
    {
        DebugIt("Player Connected with ID: " + id.ToString());
        RpcId(id, "res://Scripts/Multiplayer/ClientBootstrapping.cs"); // get the name of the player that connected
    }


    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false)]
    public void SetPlayerName(string name, long id)
    {
        DebugIt("SetPlayerName called from server");
        SavePlayer(name, Multiplayer.GetRemoteSenderId()); // save the player information in the GameManager
        Rpc("SavePlayer", name, Multiplayer.GetRemoteSenderId());
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void GetPlayerName() { /* leer – Client übernimmt */ }

    private void SavePlayer(string name, long id)
    {
        var newPlayer = new PlayerInfo { Name = name, Id = (int)id };
        if (!GameManager.Players.Any(player => player.Id == newPlayer.Id))
        {
            GameManager.Players.Add(newPlayer);
            DebugIt((Multiplayer.GetUniqueId().ToString(), " added -> Name: ", name, " id: ", id).ToString());
        }

        DebugIt(("Totale of ", GameManager.Players.Count, " players connected").ToString());

        //var label = GetParent().GetNode<RichTextLabel>("CurrentPlayers");
        //label.Text = "Start a game with:\n" + string.Join("\n", GameManager.Players.Select(player => $"Name: {player.Name} with ID: {player.Id}"));
    }

    private void DeletePlayer(long id)
    {
        var playerToRemove = GameManager.Players.FirstOrDefault(player => player.Id == id);
        if (playerToRemove != null)
        {
            GameManager.Players.Remove(playerToRemove);
            DebugIt((Multiplayer.GetUniqueId().ToString(), " removed -> id: ", id).ToString());
        }
    }


    private string GetServerIPAddress()
    {
        try
        {
            // Create a dummy UDP socket to determine the local IP address being used to access the internet
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0)) // 0 = default protocol
            {
                // Set the socket to non blocking mode
                socket.Blocking = false;

                // Bind the socket to any available local IP address and a random port
                socket.Bind(new IPEndPoint(IPAddress.Any, 0));
                {
                    // Connect to an external IP (doesnt have to be reachable), used only to let the OS determine the correct local IP
                    socket.Connect("8.8.8.8", 65530); // Googles public DNS IP

                    // Get the local endpoint (IP and port) of the socket after connecting
                    var endPoint = socket.LocalEndPoint as IPEndPoint;

                    // Extract the local IP address from the endpoint
                    string ip = endPoint?.Address.ToString() ?? "127.0.0.1";

                    return ip;
                }
            }
        }
        catch (Exception e)
        {
            GD.PrintErr("Error getting active IP address: " + e.Message);
            // Fallback to localhost if something goes wrong
            return "127.0.0.1";
        }
    }


    public string getHostAddress()
    {
        return address;
    }


    private void DebugIt(string message)
    {
        if (enableDebug)
        {
            Debug.Print("Server: " + message);
        }
    }
}