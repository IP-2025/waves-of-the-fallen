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
    NetworkManager.Instance.InitClient(ipIO.Text);

    // disable join and host button
    joinButton.Visible = false;
    joinButton.Disabled = true;

    hostButton.Visible = false;
    hostButton.Disabled = true;
  }


  private void _on_host_button_pressed()
  {
    NetworkManager.Instance.InitServer();
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
    // change scene to game
    var gameScene = GD.Load<PackedScene>("res://Scenes/GameRoot/GameRoot.tscn");
    gameScene.Instantiate<Node>();
    GetTree().ChangeSceneToPacked(gameScene);
}


  private void DebugIt(string message)
  {
    if (enableDebug) Debug.Print("Local Menue: " + message);
  }
}