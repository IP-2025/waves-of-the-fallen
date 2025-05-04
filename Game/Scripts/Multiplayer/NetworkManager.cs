using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

// Autoload-Node: manages Netzwerk, Tick-Loop
public partial class NetworkManager : Node
{
    public bool enableDebug = true;
    [Export] public int RPC_PORT = 9999;   // ENet for RPC
    [Export] public int UDP_PORT = 3000;   // PacketPeerUDP for game data

    // peers
    private PacketPeerUdp _udpClientPeer;
    private UdpServer _udpServer;
    private ENetMultiplayerPeer _rpcServerPeer;
    private ENetMultiplayerPeer _rpcClientPeer;
    private List<PacketPeerUdp> _udpPeers = new();

    // state & queues
    public bool _isServer = false;
    public bool _isHost = false;
    private bool _gameRunning = false;
    private Queue<Command> _incomingCommands = new();
    private int _tick = 0;
    private double _acc = 0;
    private const float TICK_DELTA = 1f / 60f;
    public static NetworkManager Instance { get; private set; }

    private static readonly Dictionary<string, EntityType> ScenePathToEntityType = new()
{
    { "res://Scenes/Characters/default_player.tscn", EntityType.DefaultPlayer },
    { "res://Scenes/Characters/archer.tscn", EntityType.Archer },
    { "res://Scenes/Characters/default_enemy.tscn", EntityType.DefaultEnemy },
    { "res://Scenes/Characters/ranged_enemy.tscn", EntityType.RangedEnemy }
};


    private Client client;
    private Server server;


    public override void _Ready()
    {
        Instance = this;
    }

    public void InitServer()
    {
        // add node as child to NetworkManager
        server = new Server();
        AddChild(server);

        // RPC Server with ENet
        _rpcServerPeer = new ENetMultiplayerPeer();
        // test if address and port is valid / open
        var err = _rpcServerPeer.CreateServer(RPC_PORT, maxClients: 4); // max 4 clients
        DebugIt($"ENet CreateClient: {err}");

        if (err != Error.Ok)
        {
            DebugIt("Failed to create RPC server, quitting game." + err);
            GetTree().Quit();
            return;
        }

        GetTree().GetMultiplayer().MultiplayerPeer = _rpcServerPeer;

        // UDP_PORT Server for game data
        _udpServer = new UdpServer();
        err = _udpServer.Listen((ushort)UDP_PORT);
        if (err != Error.Ok)
        {
            DebugIt("Failed to bind UDP port: " + err);
            GetTree().Quit();
            return;
        }

        _isServer = true;
        DebugIt("Server startet on port: RPC " + RPC_PORT + " + UDP " + UDP_PORT + " IP: " + GetServerIPAddress());
    }

    public void InitClient(string address)
    {
        // add client node as child to NetworkManager
        client = new Client();
        AddChild(client);


        // RPC Client with ENet
        _rpcClientPeer = new ENetMultiplayerPeer();
        _rpcClientPeer.CreateClient(address, RPC_PORT);
        GetTree().GetMultiplayer().MultiplayerPeer = _rpcClientPeer;

        // UDP Client for game data
        _udpClientPeer = new PacketPeerUdp();
        var err = _udpClientPeer.ConnectToHost(address, UDP_PORT);
        if (err != Error.Ok)
        {
            GD.PrintErr($"UDP-Connect fehlgeschlagen: {err}");
            return;
        }

        // send hello handshake to server
        var hello = Encoding.UTF8.GetBytes("HELLO");
        _udpClientPeer.PutPacket(hello);

        _isServer = false;
        DebugIt("Client connecting to: RPC " + RPC_PORT + " + UDP " + UDP_PORT + " IP: " + address);
    }

    public void InitHost() // client and server for local multiplayer
    {
        _isHost = true;

        server = new Server();
        AddChild(server);
        // start rpc
        _rpcServerPeer = new ENetMultiplayerPeer();
        // test if address and port is valid / open
        var err = _rpcServerPeer.CreateServer(RPC_PORT, maxClients: 4); // max 4 clients
        if (err != Error.Ok)
        {
            DebugIt("Failed to create RPC server, quitting game." + err);
            GetTree().Quit();
            return;
        }

        // start udp server
        _udpServer = new UdpServer();
        err = _udpServer.Listen((ushort)UDP_PORT);
        if (err != Error.Ok)
        {
            DebugIt("Failed to bind UDP port: " + err);
            GetTree().Quit();
            return;
        }
        DebugIt("Server startet on port: RPC " + RPC_PORT + " + UDP " + UDP_PORT + " IP: " + "127.0.0.1");

        // connect rpc client (localhost)
        client = new Client();
        AddChild(client);
        _rpcClientPeer = new ENetMultiplayerPeer();
        err = _rpcClientPeer.CreateClient("127.0.0.1", RPC_PORT);
        if (err != Error.Ok)
        {
            DebugIt($"Host: ENet-Client failed: {err}");
            GetTree().Quit();
            return;
        }
        GetTree().GetMultiplayer().MultiplayerPeer = _rpcClientPeer;
        GetTree().GetMultiplayer().PeerConnected += id => DebugIt($"PeerConnected: {id}");
        DebugIt("Host: ENet-Client connected to: RPC " + RPC_PORT + " + UDP " + UDP_PORT + " IP: " + "127.0.0.1");


        // connect udp client (localhost)
        _udpClientPeer = new PacketPeerUdp();
        err = _udpClientPeer.ConnectToHost("127.0.0.1", UDP_PORT);
        if (err != Error.Ok)
        {
            GD.PrintErr($"UDP-Connect fehlgeschlagen: {err}");
            return;
        }


        // send hello handshake to server
        var hello = Encoding.UTF8.GetBytes("HELLO");
        _udpClientPeer.PutPacket(hello);

        // we are now both server and client
        DebugIt("Host started (Server+Client) on localhost");
    }


    public override void _PhysicsProcess(double delta)
    {
        if (!_gameRunning) return;

        // UDP Networking
        if (_isHost)
        {
            _rpcServerPeer.Poll();
            HandleServerUdp();
            HandleClientUdp();
        }
        else if (_isServer)// client or server
        {
            HandleServerUdp();
        }
        else
            HandleClientUdp();

        // fixed timestep game loop
        HandleTickLoop(delta);
    }

    private void HandleServerUdp()
    {
        // poll for new connections
        _udpServer.Poll();
        AcceptNewUdpConnections();
        // receive incoming commands
        ReceiveServerCommands();
    }

    private void AcceptNewUdpConnections()
    {
        while (_udpServer.IsConnectionAvailable())
        {
            var peer = _udpServer.TakeConnection();
            if (peer != null)
            {
                // discard initial handshake
                if (peer.GetAvailablePacketCount() > 0)
                    peer.GetPacket();
                _udpPeers.Add(peer);
            }
        }
    }

    private void ReceiveServerCommands()
    {
        foreach (var peer in _udpPeers)
        {
            while (peer.GetAvailablePacketCount() > 0)
            {
                var data = peer.GetPacket();
                var text = Encoding.UTF8.GetString(data);
                if (text == "HELLO")
                {
                    DebugIt($"Handshake von {peer.GetPacketIP()}:{peer.GetPacketPort()} erhalten");
                    continue;
                }

                // deserialize command
                var cmd = Serializer.Deserialize<Command>(data);
                _incomingCommands.Enqueue(cmd);
            }
        }
    }

    private void HandleClientUdp()
    {
        while (_udpClientPeer.GetAvailablePacketCount() > 0)
        {
            var data = _udpClientPeer.GetPacket();
            var text = Encoding.UTF8.GetString(data);

            if (text == "START")
            {
                DebugIt("Received START packet → switching to Game scene");
                CallDeferred(nameof(NotifyGameStart)); // oder client.RpcLocal?
                continue;
            }


            var snap = Serializer.Deserialize<Snapshot>(data);
            client.ApplySnapshot(snap);
            DebugIt($"Received snapshot tick={snap.Tick}, entities={snap.Entities.Count}");
        }
    }

    private void HandleTickLoop(double delta)
    {
        _acc += delta;
        while (_acc >= TICK_DELTA)
        {
            _acc -= TICK_DELTA;
            if (_isServer)
                ProcessServerTick();
            else
                SendClientCommand();
            _tick++;
        }
    }


    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void NotifyGameStart()
    {
        // change scene to game
        var gameScene = GD.Load<PackedScene>("res://Scenes/GameRoot/GameRoot.tscn");
        gameScene.Instantiate<Node>();
        GetTree().ChangeSceneToPacked(gameScene);
        DebugIt("NotifyGameStart called: Scene changed, game running set to true.");

        _gameRunning = true;
    }

    public void BroadcastGameStartOverUDP()
    {
        var startPacket = Encoding.UTF8.GetBytes("START");
        foreach (var peer in _udpPeers)
            peer.PutPacket(startPacket);

        CallDeferred(nameof(NotifyGameStart));
    }
    private void ProcessServerTick()
    {
        // apply all commands
        while (_incomingCommands.Count > 0)
        {
            var cmd = _incomingCommands.Dequeue();
            Server.Instance.ProcessCommand(cmd);
        }

        // serialize snapshot
        var snap = new Snapshot(_tick);
        var toRemove = new List<long>();
        foreach (var kv in Server.Instance.Entities)
        {
            var node = kv.Value;
            if (node == null || !IsInstanceValid(node))
            {
                toRemove.Add(kv.Key);
                continue;
            }

            string scenePath = node.SceneFilePath;
            var id = kv.Key;

            if (!ScenePathToEntityType.TryGetValue(scenePath, out var entityType))
            {
                GD.PrintErr($"Unknown ScenePath: {scenePath}");
            }
            // Find WaveTimer as a child of the current camera
            var cam = GetViewport().GetCamera2D();
            var waveTimer = cam?.GetNode<WaveTimer>("WaveTimer");

            // Get health
            var healthNode = node.GetNodeOrNull<Health>("Health");
            float health = healthNode.health;
            snap.Entities.Add(new EntitySnapshot(id, node.Position, node.Rotation, health, entityType, waveTimer.waveCounter, waveTimer.secondCounter));
        }

        // remove invalid entities, for example killed enemies
        foreach (var id in toRemove)
        {
            Server.Instance.Entities.Remove(id);
        }

        byte[] bytes = Serializer.Serialize(snap);
        // send snapshot to all clients
        foreach (var peer in _udpPeers)
        {
            peer.PutPacket(bytes);
        }
    }

    private void SendClientCommand()
    {
        if (_udpClientPeer == null) return;

        Command cmd = client.GetCommand(_tick);
        if (cmd == null) return;

        _udpClientPeer.PutPacket(Serializer.Serialize(cmd));
        DebugIt($"Send MOVE cmd tick={_tick}, dir={cmd.MoveDir}");
    }

    public string GetServerIPAddress()
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
                    string ip = endPoint?.Address.ToString() ?? "127.0.0.1"; // localhost if null

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

    private void DebugIt(string message)
    {
        if (enableDebug) Debug.Print("Network Manager: " + message);
    }
}