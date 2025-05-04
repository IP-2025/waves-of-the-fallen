using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

public partial class LocalMenu : Control
{
  public bool enableDebug = true;

  private Button joinButton;
  private Button hostButton;
  private Button playButton;
  private LineEdit ipIO;
  private readonly int PORT = 9999;
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

    var mp = GetTree().GetMultiplayer();

    // 2) Binde die richtigen Signale
    mp.ConnectedToServer += OnConnectedToServer;
    mp.ConnectionFailed += OnConnectionFailed;
  }


  private void _on_button_back_local_pressed()
  {
    // TODO: disconect from server / host
    var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/online_localMenu.tscn");
    GetTree().ChangeSceneToPacked(scene);
  }


  private void _on_join_button_pressed()
  {
    NetworkManager.Instance.InitClient(ipIO.Text);

    // disable join and host button
    joinButton.Visible = false;
    joinButton.Disabled = true;

    hostButton.Visible = false;
    hostButton.Disabled = true;
  }


  private void _on_host_button_pressed()
  {
    //NetworkManager.Instance.InitHost();
    NetworkManager.Instance.InitServer();
    /*     NetworkManager server = NetworkManager.Instance;
        server.InitServer();
        NetworkManager client = NetworkManager.Instance;
        client.InitClient("127.0.0.1");
     */
    ipIO.Text = NetworkManager.Instance.GetServerIPAddress(); // show server ip in input field

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

    //NetworkManager.Instance.BroadcastGameStartOverUDP();

    NetworkManager.Instance.Rpc(nameof(NetworkManager.NotifyGameStart));

  }

  private void OnConnectedToServer()
  {
    DebugIt("Sucessfully conneted with srver");
  }

  private void OnConnectionFailed()
  {
    DebugIt("Connection to server failed");
    // UI zurücksetzen
    joinButton.Visible = true;
    joinButton.Disabled = false;
    hostButton.Visible = true;
    hostButton.Disabled = false;
  }


  private void DebugIt(string message)
  {
    if (enableDebug) Debug.Print("Local Menue: " + message);
  }
}