using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public partial class LocalMenu : Control
{
  public bool enableDebug = false;

  private Button joinButton;
  private Button hostButton;
  private Button playButton;
  private LineEdit ipIO;
  private RichTextLabel currentPlayers;
  //private ServerBootstrapping server;


  public override void _Ready()
  {
    joinButton = GetNode<Button>("MarginContainer2/VBoxContainer/MarginContainer/HBoxContainer/join");
    hostButton = GetNode<Button>("MarginContainer2/VBoxContainer/MarginContainer/HBoxContainer/host");
    playButton = GetNode<Button>("MarginContainer2/VBoxContainer/MarginContainer/HBoxContainer/play");
    ipIO = GetNode<LineEdit>("IP_IO");
    currentPlayers = GetNode<RichTextLabel>("CurrentPlayers");

    // disable play button by default
    playButton.Visible = false;
    playButton.Disabled = true;
  }

  public override void _Process(double delta)
  {
    var peers = Multiplayer.GetPeers().ToList();

    currentPlayers.Text = $"players: {peers.Count}\n" +
                          string.Join("\n", peers.Select(id => $"ID {id}"));
  }

  private void _on_button_back_local_pressed()
  {
    // TODO: disconect from server / host
    var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/online_localMenu.tscn");
    GetTree().ChangeSceneToPacked(scene);
  }


  private void _on_join_button_pressed()
  {
    string ipv4Pattern = @"^(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)){3}$";
    if (!Regex.IsMatch(ipIO.Text, ipv4Pattern)) return;

    NetworkManager.Instance.InitClient(ipIO.Text);

    // disable join and host button
    joinButton.Visible = false;
    joinButton.Disabled = true;

    hostButton.Visible = false;
    hostButton.Disabled = true;
  }


  private void _on_host_button_pressed()
  {
    ipIO.Text = NetworkManager.Instance.GetServerIPAddress(); // show server ip in input field

    NetworkManager.Instance.StartHeadlessServer(true);

    if (NetworkManager.Instance.IsPortOpen(NetworkManager.Instance.GetServerIPAddress(), NetworkManager.Instance.RPC_PORT, 500))
    {
      NetworkManager.Instance.InitClient(NetworkManager.Instance.GetServerIPAddress());
    }
    else
    {
      var timer = new Timer();
      AddChild(timer);
      timer.WaitTime = 1;
      timer.OneShot = true;
      timer.Timeout += () => NetworkManager.Instance.InitClient(NetworkManager.Instance.GetServerIPAddress());
      timer.Start();
    }

    // disable host and join button and enable play button
    hostButton.Visible = false;
    hostButton.Disabled = true;

    joinButton.Visible = false;
    joinButton.Disabled = true;

    playButton.Visible = true;
    playButton.Disabled = false;

  }

  private void _on_play_button_pressed()
  {
    NetworkManager.Instance.Rpc("NotifyGameStart");
  }

  private void DebugIt(string message)
  {
    if (enableDebug) Debug.Print("Local Menue: " + message);
  }
}