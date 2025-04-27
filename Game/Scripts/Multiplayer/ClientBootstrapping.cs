using System;
using System.Linq;
using Godot;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Runtime.ConstrainedExecution;

// Tis is for online multiplayer. The server simulates the multiplayer setup the host player dose in a local game
public partial class ClientBootstrapping : Node
{
    private int port;
    private string address;
    private ENetMultiplayerPeer peer;
    public bool enableDebug = true;

    public override void _Ready()
    {
    }


    public void Init(int port, string address)
    {
        this.port = port;
        this.address = address;

        peer = new ENetMultiplayerPeer();
        peer.CreateClient(address, port);
        Multiplayer.MultiplayerPeer = peer;

        DebugIt("Instantiating local multiplayer");
        Name = Multiplayer.GetUniqueId().ToString(); // later from line edit

        MultiplayerSynchronizer multiplayerSynchronizer = new MultiplayerSynchronizer();
    }


    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    public void GetPlayerName()
    {
        // this is called from server to get player information like name
        DebugIt("GetPlayerInformation called from server");
        RpcId(1, "SetPlayerName", Name);
    }


    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false)]
    public void SavePlayer(string name, long id)
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

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false)]
    public void DeletePlayer(long id)
    {
        var playerToRemove = GameManager.Players.FirstOrDefault(player => player.Id == id);
        if (playerToRemove != null)
        {
            GameManager.Players.Remove(playerToRemove);
            DebugIt((Multiplayer.GetUniqueId().ToString(), " removed -> id: ", id).ToString());
        }
    }
    private void DebugIt(string message)
    {
        if (enableDebug)
        {
            Debug.Print("Client: " + message);
        }
    }
}