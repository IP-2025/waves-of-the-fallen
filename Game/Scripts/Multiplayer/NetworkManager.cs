using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;

// Autoload-Node: manages network stuff, Tick-Loop
public partial class NetworkManager : Node
{
    public bool enableDebug = true;
    private readonly int PORT = 5000;
    private ENetMultiplayerPeer _peer;
    private Queue<Command> _incomingCommand = new Queue<Command>();
    private int _sequence = 0;
    private int _tick = 0;
    private double _accumulator = 0;
    private const float TickDelta = 1f / 20f;
    private bool _gameRunning = false;


    public static NetworkManager Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this;
    }

    public void InitServer()
    {
        _peer = new ENetMultiplayerPeer();
        _peer.CreateServer(PORT, 4); // max 4 clients
        GetTree().GetMultiplayer().MultiplayerPeer = _peer;
        DebugIt("Server startet on port: " + PORT + " IP: " + GetServerIPAddress());
    }

    public void InitClient(string address)
    {
        _peer = new ENetMultiplayerPeer();

        // test if address and port is valid / open
        var err = _peer.CreateClient(address, PORT);
        DebugIt($"ENet CreateClient: {err}");

        if (err != Error.Ok)
        {
            DebugIt("Failed to create client, quitting game.");
            GetTree().Quit();
            return;
        }

        GetTree().GetMultiplayer().MultiplayerPeer = _peer;
        DebugIt("Client connecting to: " + address + ":" + PORT);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_gameRunning) return;

        _accumulator += delta;
        while (_accumulator >= TickDelta)
        {
            _accumulator -= TickDelta;
            ProcessNetwork(); // process incoming packets
            if (GetTree().GetMultiplayer().IsServer())
            {
                UpdateGame();
                SendSnapshot();
            }
            else
            {
                // TODO: SendCommand();
            }

            _tick++;
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void NotifyGameStart()
    {

        StartGame();
    }

    private void StartGame()
    {
        if (GetTree().GetMultiplayer().IsServer())
        {
            Server.Instance.InitServer();
        }

        _gameRunning = true;
    }

    private void ProcessNetwork()
    {
        var mp = GetTree().GetMultiplayer().GetMultiplayerPeer() as ENetMultiplayerPeer;

        int cnt = mp.GetAvailablePacketCount();
        DebugIt($"{GetTree().GetMultiplayer().GetUniqueId()} Incoming packets: {cnt}");

        while (cnt > 0)
        {
            var pkt = mp.GetPacket();
            if (GetTree().GetMultiplayer().IsServer())
            {
                // Server only receives commands
                var cmd = Serializer.Deserialize<Command>(pkt);
                _incomingCommand.Enqueue(cmd);
            }
            else
            {
                // Client only receives snapshots
                var snap = Serializer.Deserialize<Snapshot>(pkt);
                // handle snapshots to player manager
                GetTree().Root
                            .GetNode<GameRoot>("GameRoot")
                            .GetNode<Client>("Client")
                            .ApplySnapshot(snap);
            }
        }
    }


    private void UpdateGame()
    {
        while (_incomingCommand.Any())
        {
            var cmd = _incomingCommand.Dequeue();
            Server.Instance.ProcessCommand(cmd);
        }
    }

    private void SendSnapshot()
    {
        var snap = new Snapshot(_tick);
        foreach (var kv in Server.Instance.Entities)
        {
            snap.Entities.Add(new EntitySnapshot(kv.Key, kv.Value.Position, kv.Value.Rotation));
        }

        //prodcast to all clients
        var mp = GetTree().GetMultiplayer().GetMultiplayerPeer() as ENetMultiplayerPeer;
        if (mp == null) return;
        mp.SetTargetPeer((int)MultiplayerPeer.TargetPeerBroadcast);
        mp.PutPacket(Serializer.Serialize(snap));
        DebugIt($"Server: sending snapshot tick={_tick}");
    }

    public void SendCommand(Command cmd)
    {
        cmd.Sequence = _sequence++;

        // prodcast to server only
        var mp = GetTree().GetMultiplayer().GetMultiplayerPeer() as ENetMultiplayerPeer;
        if (mp == null) return;
        mp.SetTargetPeer((int)MultiplayerPeer.TargetPeerServer);
        mp.PutPacket(Serializer.Serialize(cmd));
        DebugIt($"Client: Incoming packets: {mp.GetAvailablePacketCount()}");
    }

    public static float GetTickDelta()
    {
        return TickDelta;
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