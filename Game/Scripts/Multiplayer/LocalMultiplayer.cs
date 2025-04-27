using System;
using System.Linq;
using Godot;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
public partial class LocalMultiplayer : Control
{
    private int port = 9999; // multiplayer port
    private string address;
    private ENetMultiplayerPeer peer;
    public bool enableDebug = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        DebugIt("Instantiating local multiplayer");
        Name = Multiplayer.GetUniqueId().ToString();

        MultiplayerSynchronizer multiplayerSynchronizer = new MultiplayerSynchronizer();
    }

    
    public void Host()
    {
        //Server server = new Server(port);
        //address = server.getHostAddress();

        //Client client = new Client(port, address);
        
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

        // if host, show all players in the lobby in label in LocalMenu

            var label =  GetParent().GetNode<RichTextLabel>("CurrentPlayers");
            label.Text = "Start a game with:\n" + string.Join("\n", GameManager.Players.Select(player => $"Name: {player.Name} with ID: {player.Id}"));

        
    }

    private string GetThisIPAddress()
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

                    DebugIt("Selected Active IP Address: " + ip);
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
            Debug.Print(message);
        }
    }
}
