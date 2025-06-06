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
using Game.Utilities.Multiplayer;

public partial class LocalMenu : Control
{
  public bool enableDebug = false;

  private Button joinButton;
  private Button hostButton;
  private Button playButton;
  private LineEdit ipIO;
  private bool isHost = false;
  private RichTextLabel currentPlayers;
  //private ServerBootstrapping server;


  public override void _Ready()
  {
    GD.Print("LocalMenu: Ready aufgerufen – isHost=" + isHost);
    joinButton = GetNode<Button>("MarginContainer2/VBoxContainer/MarginContainer/HBoxContainer/join");
    hostButton = GetNode<Button>("MarginContainer2/VBoxContainer/MarginContainer/HBoxContainer/host");
    playButton = GetNode<Button>("MarginContainer2/VBoxContainer/MarginContainer/HBoxContainer/play");
    ipIO = GetNode<LineEdit>("IP_IO");
    currentPlayers = GetNode<RichTextLabel>("CurrentPlayers");
    NetworkManager.Instance.HeadlessServerInitialized += OnHeadlessServerInitialized;

    // disable play button by default
    playButton.Visible = false;
    playButton.Disabled = true;
  }

  public override void _Process(double delta)
  {
    if (!isHost) return;

    var peers = Multiplayer.GetPeers().ToList();

    currentPlayers.Text = $"Players: {peers.Count}\n" + string.Join("\n", peers.Select(id => $"ID {id}"));
  }

  private void _on_button_back_local_pressed()
  {
    // 1. Disconnect vom Server/Host
    NetworkManager.Instance.DisconnectClient();
    // 2. UI-Sound
    SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
    // 3. Neue Szene laden – Godot killt automatisch alle alten Nodes + Scripte
    var scene = ResourceLoader.Load<PackedScene>("res://Menu/online_localMenu.tscn");
    var err = GetTree().ChangeSceneToPacked(scene);
    if (err != Error.Ok)
      GD.PrintErr($"Konnte Szene nicht wechseln: {err}");
  }


  private void _on_join_button_pressed()
  {
    string ipv4Pattern = @"^(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)){3}$";
    if (!Regex.IsMatch(ipIO.Text, ipv4Pattern)) return;

    NetworkManager.Instance.InitClient(ipIO.Text);

    var characterManager = GetNode<CharacterManager>("/root/CharacterManager");
    int selectedCharacterId = characterManager.LoadLastSelectedCharacterID();

    var timer2 = new Timer();
    AddChild(timer2);
    timer2.WaitTime = 0.5f;
    timer2.OneShot = true;
    timer2.Timeout += () => NetworkManager.Instance.RpcId(1, "SelectCharacter", selectedCharacterId);
    timer2.Start();

    // disable join and host button
    joinButton.Visible = false;
    joinButton.Disabled = true;

    hostButton.Visible = false;
    hostButton.Disabled = true;
    SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
  }


  private void _on_host_button_pressed()
  {
    hostButton.Disabled = true;

    joinButton.Visible = false;
    joinButton.Disabled = true;

    isHost = true;

    GD.Print("Start Headless");
    NetworkManager.Instance.StartHeadlessServer(true);
    GD.Print("Headless started");
    SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
  }

  private void _on_play_button_pressed()
  {

    var characterManager = GetNode<CharacterManager>("/root/CharacterManager");
    int selectedCharacterId = characterManager.LoadLastSelectedCharacterID();
    NetworkManager.Instance.RpcId(1, "SelectCharacter", selectedCharacterId);
    NetworkManager.Instance.Rpc("NotifyGameStart");
    SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
  }

  private void DebugIt(string message)
  {
    if (enableDebug) Debug.Print("Local Menue: " + message);
  }

  private void OnHeadlessServerInitialized()
  {
    var timer = new Timer();
    AddChild(timer);
    timer.WaitTime = 0.5f;
    timer.OneShot = true;
    timer.Timeout += () =>
    {
      NetworkManager.Instance.InitClient(NetworkManager.Instance.GetServerIPAddress());
      ipIO.Text = NetworkManager.Instance.GetServerIPAddress(); // show Server IP

      var timer2 = new Timer();
      AddChild(timer2);
      timer2.WaitTime = 0.5f;
      timer2.OneShot = true;
      timer2.Timeout += () =>
      {
        hostButton.Visible = false;
        playButton.Visible = true;
        playButton.Disabled = false;
      };

      timer2.Start();
    };

    timer.Start();
  }


  public override void _ExitTree()
  {
    NetworkManager.Instance.HeadlessServerInitialized -= OnHeadlessServerInitialized;
  }
}