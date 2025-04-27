using Godot;
using System.Collections.Generic;
using System.Linq;

// Autoload-Node: verwaltet Netzwerk, Tick-Loop
public partial class NetworkManager : Node
{
    [Export] public int Port = 9999;
    private ENetMultiplayerPeer _peer;
    private Queue<Command> _incoming = new Queue<Command>();
    private int _sequence = 0;
    private int _tick = 0;
    private double _accumulator = 0;
    private const float TickDelta = 1f / 20f;

    public override void _Ready()
    {
        // InitServer() oder InitClient() manuell aufrufen
    }

    public void InitServer()
    {
        _peer = new ENetMultiplayerPeer();
        _peer.CreateServer(Port, 16);
        GetTree().GetMultiplayer().MultiplayerPeer = _peer;
        GD.Print("Server gestartet auf Port " + Port);
    }

    public void InitClient(string address)
    {
        _peer = new ENetMultiplayerPeer();
        _peer.CreateClient(address, Port);
        GetTree().GetMultiplayer().MultiplayerPeer = _peer;
        GD.Print("Client verbindet an " + address + ":" + Port);
    }

    public override void _PhysicsProcess(double delta)
    {
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

    private void ProcessNetwork()
    {
        while (_peer.GetPeer(0).GetAvailablePacketCount() > 0)
        {
            var pkt = _peer.GetPeer(0).GetPacket();
            var cmd = Serializer.Deserialize<Command>(pkt);
            _incoming.Enqueue(cmd);
        }
    }

    private void UpdateGame()
    {
        while (_incoming.Any())
        {
            var cmd = _incoming.Dequeue();
            GameManager2.Instance.ProcessCommand(cmd);
        }
    }

    private void SendSnapshot()
    {
        var snap = new Snapshot(_tick);
        foreach (var kv in GameManager2.Instance.Entities)
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
}