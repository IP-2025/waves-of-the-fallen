using Godot;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Game.Utilities.Multiplayer;

public partial class LocalMenu : Control
{
	private bool _enableDebug = false;

	private Button _joinButton;
	private Button _hostButton;
	private Button _playButton;
	private LineEdit _ipIo;
	private bool _isHost = false;
	private RichTextLabel _currentPlayers;
	//private ServerBootstrapping server;


	public override void _Ready()
	{
		MultiplayerCleanup mc = new MultiplayerCleanup();
		mc.StartFullCleanup(GetTree().GetMultiplayer());
		GD.Print("LocalMenu: Ready aufgerufen – isHost=" + _isHost);
		_joinButton = GetNode<Button>("%join");
		_hostButton = GetNode<Button>("%host");
		_playButton = GetNode<Button>("%play");
		_ipIo = GetNode<LineEdit>("%IP_IO");
		_currentPlayers = GetNode<RichTextLabel>("%CurrentPlayers");

		// disable play button by default
		_playButton.Visible = false;
		_playButton.Disabled = true;
	}

	public override void _Process(double delta)
	{
		if (!_isHost || Multiplayer.MultiplayerPeer == null) return;

		var peers = Multiplayer.GetPeers().ToList();

		_currentPlayers.Text = $"Players playing with you: {peers.Count}\n" + string.Join("\n", peers.Select(id => $"ID {id}"));
	}

	private void _on_button_back_local_pressed()
	{
		MultiplayerCleanup mc = new MultiplayerCleanup();
		mc.StartFullCleanup(GetTree().GetMultiplayer());
		mc.QueueFree();
		SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
		// Neue Szene laden – Godot killt automatisch alle alten Nodes + Scripte
		var scene = ResourceLoader.Load<PackedScene>("res://Menu/online_localMenu.tscn");
		var timer = new Timer();
		AddChild(timer);
		timer.WaitTime = 0.2f;
		timer.OneShot = true;
		timer.Timeout += () =>
		{
			var err = GetTree().ChangeSceneToPacked(scene);
			if (err != Error.Ok)
				GD.PrintErr($"Cant change scene: {err}");
		};
		timer.Start();
	}


	private void _on_join_button_pressed()
	{
		const string codePattern = @"^\d{1,3}$";
		if (!Regex.IsMatch(_ipIo.Text, codePattern)) return;

		NetworkManager.Instance.InitClient(_ipIo.Text);

		var characterManager = GetNode<CharacterManager>("/root/CharacterManager");
		var selectedCharacterId = characterManager.LoadLastSelectedCharacterID();
		var health = characterManager.LoadHealthByID(selectedCharacterId.ToString());
		var speed = characterManager.LoadSpeedByID(selectedCharacterId.ToString());
		var dexterity = characterManager.LoadDexterityByID(selectedCharacterId.ToString());
		var strength = characterManager.LoadStrengthByID(selectedCharacterId.ToString());
		var intelligence = characterManager.LoadIntelligenceByID(selectedCharacterId.ToString());

		var timer2 = new Timer();
		AddChild(timer2);
		timer2.WaitTime = 0.5f;
		timer2.OneShot = true;
		timer2.Timeout += () => NetworkManager.Instance.RpcId(1, "SelectCharacter", selectedCharacterId, health, speed);
		timer2.Start();

		// disable join and host button
		_joinButton.Visible = false;
		_joinButton.Disabled = true;

		_hostButton.Visible = false;
		_hostButton.Disabled = true;
		SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
	}


	private void _on_host_button_pressed()
	{
		_hostButton.Disabled = true;
		_hostButton.Visible = false;
		_joinButton.Visible = false;
		_joinButton.Disabled = true;
		_isHost = true;

		NetworkManager.Instance.InitServer();
		NetworkManager.Instance._isLocalHost = true;
		// small delay...
		var timer = new Timer();
		AddChild(timer);
		timer.WaitTime = 0.5f;
		timer.OneShot = true;
		timer.Timeout += () => { };
		timer.Start();

		_ipIo.Text = NetworkManager.Instance.GenerateConnectionCode();
		//NetworkManager.Instance.GetServerIPAddress(); // show Server IP

		//ipIO.Text = "Running";
		_playButton.Visible = true;
		_playButton.Disabled = false;

		_currentPlayers.SizeFlagsVertical = Control.SizeFlags.Expand;
		_currentPlayers.AutowrapMode = TextServer.AutowrapMode.Word;
		_currentPlayers.CustomMinimumSize = new Vector2(0, 100); 

		SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
	}

	private void _on_play_button_pressed()
	{
		var characterManager = GetNode<CharacterManager>("/root/CharacterManager");
		int selectedCharacterId = characterManager.LoadLastSelectedCharacterID();
		var health = characterManager.LoadHealthByID(selectedCharacterId.ToString());
		var speed = characterManager.LoadSpeedByID(selectedCharacterId.ToString());
		NetworkManager.Instance.RpcId(1, "SelectCharacter", selectedCharacterId, health, speed);
		NetworkManager.Instance.Rpc("NotifyGameStart");
		SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
	}

	private void DebugIt(string message)
	{
		if (_enableDebug) Debug.Print("Local Menue: " + message);
	}

	public override void _Notification(int what)
	{
		if (what == NotificationWMGoBackRequest)
		{
			_on_button_back_local_pressed();
		}
	}
}
