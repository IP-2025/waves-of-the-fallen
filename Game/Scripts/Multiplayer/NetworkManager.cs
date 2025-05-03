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
    public bool enableDebug = true;
    // portsorts
    [Export] public int RPC_PORT = 7777;   // ENet for RPC
    [Export] public int UDP_PORT = 3000;   // PacketPeerUDP for game data
    // peers
    private ENetMultiplayerPeer _rpcPeer;
    private PacketPeerUdp _udpPeer;
    // state & queues
    private bool _isServer = false;
    private bool _gameRunning = false;
    private Queue<Command> _incomingCommands = new();
    private HashSet<(string ip, int port)> _clients = new();
    private int _sequence = 0;
    private int _tick = 0;
    private double _acc = 0;
    private const float TICK_DELTA = 1f / 20f;
    private double _accumulator = 0;
    public static NetworkManager Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    public void InitServer()
    {
        // RPC Server with ENet
        _rpcPeer = new ENetMultiplayerPeer();
        // test if address and port is valid / open
        var err = _rpcPeer.CreateServer(RPC_PORT, maxClients: 4); // max 4 clients
        DebugIt($"ENet CreateClient: {err}");

        if (err != Error.Ok)
        {
            DebugIt("Failed to create RPC server, quitting game." + err);
            GetTree().Quit();
            return;
        }

        GetTree().GetMultiplayer().MultiplayerPeer = _rpcPeer;

        // UDP_PORT Server for game data
        _udpPeer = new PacketPeerUdp();
        err = _udpPeer.Bind(UDP_PORT);
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
        // RPC Client with ENet
        _rpcPeer = new ENetMultiplayerPeer();
        _rpcPeer.CreateClient(address, RPC_PORT);
        GetTree().GetMultiplayer().MultiplayerPeer = _rpcPeer;

        // UDP Client for game data
        _udpPeer = new PacketPeerUdp();
        var err = _udpPeer.ConnectToHost(address, UDP_PORT);
        if (err != Error.Ok)
        {
            GD.PrintErr($"UDP-Connect fehlgeschlagen: {err}");
            return;
        }

        _isServer = false;
        DebugIt("Client connecting to: RPC " + RPC_PORT + " + UDP " + UDP_PORT + " IP: " + address);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_gameRunning) return;
        // always process udp packets first rec
        while (_udpPeer.GetAvailablePacketCount() > 0)
        {
            byte[] data = _udpPeer.GetPacket();
            if (_isServer)
            {
                var cmd = Serializer.Deserialize<Command>(data);
                _incomingCommands.Enqueue(cmd);
                DebugIt($"CMD receiverd seq={cmd.Sequence}");
            }
            else
            {
                var snap = Serializer.Deserialize<Snapshot>(data);
//                GetNode<Client>("Client").ApplySnapshot(snap);
                DebugIt($"Snapshot receiverd tick={snap.Tick}");
            }
        }

        // fixed timestep loop
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
        _gameRunning = true;
    }

    private void ProcessServerTick()
    {
        // apply all incoming commands
        while (_incomingCommands.Count > 0)
        {
            string ip = _udpPeer.GetPacketIP();
            int port = _udpPeer.GetPacketPort();
            byte[] data = _udpPeer.GetPacket();

            // save all clients because udp is stateless and later we need to send snapshots to all clients
            if (!_clients.Contains((ip, port)))
            {
                _clients.Add((ip, port));
            }

            var cmd = _incomingCommands.Dequeue();
            Server.Instance.ProcessCommand(cmd);
        }

        // broadcast current state snapshot with UDP
        var snap = new Snapshot(_tick);
        foreach (var kv in Server.Instance.Entities)
            snap.Entities.Add(new EntitySnapshot(kv.Key, kv.Value.Position, kv.Value.Rotation));

        byte[] bytes = Serializer.Serialize(snap);

        foreach (var (ip, port) in _clients)
        {
            _udpPeer.SetDestAddress(ip, port);
            _udpPeer.PutPacket(bytes);
        }

        DebugIt($"Send snapshot tick={_tick}");
    }

    // client tick: create and send commands
    private void SendClientCommand()
    {
        // exaample....
        int seq = _sequence++;
        long eid = 1; // entity id
        CommandType type = CommandType.Move;
        Vector2 dir = new Vector2(1, 0);  // example direction

        // with direction
        var cmdMove = new Command(seq, eid, type, dir);
        _udpPeer.PutPacket(Serializer.Serialize(cmdMove));
        DebugIt($"Send MOVE cmd seq={cmdMove.Sequence}");

        // wthout direction
        var cmdShoot = new Command(_sequence++, eid, CommandType.Shoot);
        _udpPeer.PutPacket(Serializer.Serialize(cmdShoot));
        DebugIt($"Send SHOOT cmd seq={cmdShoot.Sequence}");
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