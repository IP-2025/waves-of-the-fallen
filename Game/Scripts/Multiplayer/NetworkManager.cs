using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;

// Autoload-Node: verwaltet Netzwerk, Tick-Loop
public partial class NetworkManager : Node
{
    public bool enableDebug = false;
    [Export] public int Port = 9999;
    private ENetMultiplayerPeer _peer;
    private Queue<Command> _incoming = new Queue<Command>();
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
        _peer.CreateServer(Port, 4); // max 4 clients
        GetTree().GetMultiplayer().MultiplayerPeer = _peer;
        DebugIt("Server startet on port: " + Port + " IP: " + GetServerIPAddress());
    }

    public void InitClient(string address)
    {
        _peer = new ENetMultiplayerPeer();
        _peer.CreateClient(address, Port);
        GetTree().GetMultiplayer().MultiplayerPeer = _peer;
        DebugIt("Client connecting to: " + address + ":" + Port);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_gameRunning) return;

        _accumulator += delta;
        while (_accumulator >= TickDelta)
        {
            _accumulator -= TickDelta;
            ProcessNetwork();
            if (GetTree().GetMultiplayer().IsServer())
            {
                UpdateGame();
            }

            SendSnapshot();
            _tick++;
        }
    }

    public void StartGame()
    {
        _gameRunning = true;
    }

    private void ProcessNetwork()
    {
        var mp = GetTree().GetMultiplayer().GetMultiplayerPeer() as ENetMultiplayerPeer;
        if (mp == null) return;

        mp.SetTargetPeer((int)MultiplayerPeer.TargetPeerServer); // nur zur Sicherheit
        while (mp.GetAvailablePacketCount() > 0)
        {
            var pkt = mp.GetPacket();
            if (GetTree().GetMultiplayer().IsServer())
            {
                // Server empfängt nur Commands
                var cmd = Serializer.Deserialize<Command>(pkt);
                _incoming.Enqueue(cmd);
            }
            else
            {
                // Client empfängt nur Snapshots
                var snap = Serializer.Deserialize<Snapshot>(pkt);
                // Snapshot an den PlayerManager geben
                var pm = GetTree().Root
                            .GetNode<GameRoot>("GameRoot")
                            .GetNode<PlayerManager>("PlayerManager");
                pm.ApplySnapshot(snap);
            }
        }
    }


    private void UpdateGame()
    {
        while (_incoming.Any())
        {
            var cmd = _incoming.Dequeue();
            GameManager.Instance.ProcessCommand(cmd);
        }
    }

    private void SendSnapshot()
    {
        var snap = new Snapshot(_tick);
        foreach (var kv in GameManager.Instance.Entities)
        {
            long netId = kv.Key;
            Node2D entity = kv.Value;
            snap.Entities.Add(new EntitySnapshot(netId,
                entity.Position, entity.Rotation));
        }

        var data = Serializer.Serialize(snap);
        BroadcastMessage(data);
    }

    public void SendCommand(Command cmd)
    {
        cmd.Sequence = _sequence++;
        var data = Serializer.Serialize(cmd);
        BroadcastMessage(data); // 1 = Server ID
    }

    void BroadcastMessage(byte[] data)
    {
        var mp = GetTree().GetMultiplayer().GetMultiplayerPeer() as ENetMultiplayerPeer;
        if (mp == null) return;
        mp.SetTargetPeer((int)MultiplayerPeer.TargetPeerBroadcast);
        mp.PutPacket(data);
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