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
  public bool enableDebug = false;

  private Button joinButton;
  private Button hostButton;
  private Button playButton;
  private LineEdit ipIO;
  private readonly int PORT = 9999;

  private RichTextLabel currentPlayers;
  private ClientBootstrapping client;
  //private ServerBootstrapping server;


  public override void _Ready()
  {
    //LocalMultiplayer = new LocalMultiplayer();
    //AddChild(LocalMultiplayer); // fügt LocalMultiplayer als Child hinzu

    joinButton = GetNode<Button>("MarginContainer2/VBoxContainer/MarginContainer/HBoxContainer/join");
    hostButton = GetNode<Button>("MarginContainer2/VBoxContainer/MarginContainer/HBoxContainer/host");
    playButton = GetNode<Button>("MarginContainer2/VBoxContainer/MarginContainer/HBoxContainer/play");
    ipIO = GetNode<LineEdit>("IP_IO");
    currentPlayers = GetNode<RichTextLabel>("CurrentPlayers");

    // disable play button by default
    playButton.Visible = false;
    playButton.Disabled = true;
  }


  private void _on_button_back_local_pressed()
  {
    // TODO: disconect from server / host
    var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/online_localMenu.tscn");
    GetTree().ChangeSceneToPacked(scene);
  }


  private void _on_join_button_pressed()
  {
    var scene = GD.Load<PackedScene>("res://Scenes/Multiplayer/Client.tscn");
    var client = scene.Instantiate<ClientBootstrapping>();
    GetTree().Root.AddChild(client);
    client.Init(PORT, ipIO.Text); // set the ip input to display local ip address

    // disable join and host button
    joinButton.Visible = false;
    joinButton.Disabled = true;

    hostButton.Visible = false;
    hostButton.Disabled = true;
  }


  private void _on_host_button_pressed()
  {
    var scene = GD.Load<PackedScene>("res://Scenes/Multiplayer/Server.tscn");
    var server = scene.Instantiate<ServerBootstrapping>();
    GetTree().Root.AddChild(server);
    server.Init(PORT);

    // create a new server and host the game
    //ipIO.Text = "Lobby address is: " + LocalMultiplayer.getHostAddress().ToString(); // set the ip input to display local ip address

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
    //LocalMultiplayer.Play();
  }


  private void DebugIt(string message)
  {
    //if (LocalMultiplayer.enableDebug)
    //{
    //  Debug.Print(message);
    // }
  }
}