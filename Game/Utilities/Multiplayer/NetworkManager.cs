//old one

using Game.Utilities.Backend;

namespace Game.Utilities.Multiplayer
{
	using Godot;
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Net;
	using System.Net.Sockets;
	using System.Text;
	using System.Threading.Tasks;

	// Autoload-Node: manages Netzwerk, Tick-Loop
	public partial class NetworkManager : Node
	{
		public bool enableDebug = true;
		public int RPC_PORT = 9999;   // ENet for RPC
		public int UDP_PORT = 3000;   // PacketPeerUDP for game data

		// peers
		private PacketPeerUdp _udpClientPeer;
		private UdpServer _udpServer;
		private ENetMultiplayerPeer _rpcServerPeer;
		private ENetMultiplayerPeer _rpcClientPeer;
		private List<PacketPeerUdp> _udpPeers = new();

		// state & queues
		public bool _isServer = false;
		private bool _gameRunning = false;
		private bool _readyForUdp = false;
		private Queue<Command> _incomingCommands = new();
		private ulong _tick = 0;
		private double _acc = 0;
		private const float TICK_DELTA = 1f / 30;
		private Timer shutdownTimer; // for headless server if no one is connected
		private const float ServerShutdownDelay = 300f; // seconds
		public static NetworkManager Instance { get; private set; }
		private Client client;
		private Server server;
		public bool _isLocalHost = false;
		public bool SoloMode = false;
		Process process = new Process();
		// Server ready signal
		[Signal]
		public delegate void HeadlessServerInitializedEventHandler();


		public override void _Ready()
		{
			enableDebug = false;
			Engine.MaxFps = 60; // this is here because NetworkManager is an autoload (and it wont work in SettingsMenu for whatever reason). This means the whole game will be on 60 fps. Workaround but works
			Instance = this;
			// check if we are in headless server mode
			var args = OS.GetCmdlineArgs();

			if (args.Contains("--server-mode"))
			{
				DebugIt("Headless Server, call InitServer()");
				CallDeferred(nameof(InitServer));
				StartAutoShutdownTimer();
			}
			GetTree().GetMultiplayer().PeerDisconnected += OnPeerDisconnected;
		}

		public override void _PhysicsProcess(double delta)
		{
			
			if (!_readyForUdp) return;

			// UDP Networking
			if (_isServer)
			{
				HandleServerUdp();
			}
			else
				HandleClientUdp();

			if (!_gameRunning) return;

			// fixed timestep game loop
			HandleTickLoop(delta);
		}

		public void DisconnectClient()
		{
			// disconnect ENetMultiplayerPeer
			if (_rpcClientPeer != null)
			{
				_rpcClientPeer.Close();
				_rpcClientPeer.Dispose();
				_rpcClientPeer = null;
				DebugIt("ENetMultiplayerPeer disconnected.");
			}

			// disconnect UDP client
			if (_udpClientPeer != null)
			{
				_udpClientPeer.Close();
				_udpClientPeer = null;
				DebugIt("UDP Client disconnected.");
			}

			// set multiplayerPeer to null
			if (GetTree().GetMultiplayer().MultiplayerPeer != null)
			{
				GetTree().GetMultiplayer().MultiplayerPeer = null;
				DebugIt("MultiplayerPeer set to null.");
			}

			// Additional cleanup
			_readyForUdp = false;
			_gameRunning = false;

			DebugIt("Client fully disconnected.");
		}

		public void InitServer()
		{
			// add node as child to NetworkManager
			server = new Server();
			AddChild(server);

			// RPC Server with ENet
			_rpcServerPeer = new ENetMultiplayerPeer();
			// test if address and port is valid / open
			var err = _rpcServerPeer.CreateServer(RPC_PORT, maxClients: _isLocalHost ? 3 : 4); // max 4 players
			DebugIt($"ENet CreateClient: {err}");

			if (err != Error.Ok)
			{
				DebugIt("Failed to create RPC server, quitting game." + err);
				GetTree().Quit();
				return;
			}

			GetTree().GetMultiplayer().MultiplayerPeer = _rpcServerPeer;

			// UDP_PORT Server for game data
			_udpServer = new UdpServer();
			err = _udpServer.Listen((ushort)UDP_PORT);
			if (err != Error.Ok)
			{
				DebugIt("Failed to bind UDP port: " + err);
				GetTree().Quit();
				return;
			}

			_isServer = true;
			_readyForUdp = true;

			DebugIt("Server startet on port: RPC " + RPC_PORT + " + UDP " + UDP_PORT + " IP: " + GetServerIPAddress());
		}

		public void InitClient(string code, int UDP_PORT = 3000, int RPC_PORT = 9999, bool isLocal = true)
		{
			string address;
			if (isLocal)
			{
				address = ResolveConnectionCode(code);
			}
			else
			{
				address = ServerConfig.ServerAddress;
			}

			// add client node as child to NetworkManager
			client = new Client();
			AddChild(client);


			// RPC Client with ENet
			_rpcClientPeer = new ENetMultiplayerPeer();
			var err1 = _rpcClientPeer.CreateClient(address, RPC_PORT);
			GD.PrintErr($"fucking rpc peer result: {err1}");
			GetTree().GetMultiplayer().MultiplayerPeer = _rpcClientPeer;

			// UDP Client for game data
			_udpClientPeer = new PacketPeerUdp();
			var err = _udpClientPeer.ConnectToHost(address, UDP_PORT);
			if (err != Error.Ok)
			{
				GD.PrintErr($"Params: udp address: {address}, udp port: {UDP_PORT}");
				GD.PrintErr($"UDP-Connect fehlgeschlagen: {err}");
				return;
			}

			// send hello handshake to server
			var hello = Encoding.UTF8.GetBytes("HELLO");
			_udpClientPeer.PutPacket(hello);

			_readyForUdp = true;
			DebugIt("Client connecting to: RPC " + RPC_PORT + " + UDP " + UDP_PORT + " IP: " + address);
		}

		private void StartAutoShutdownTimer()
		{
			if (shutdownTimer != null) return; // dont start twice

			shutdownTimer = new Timer();
			shutdownTimer.ProcessMode = Timer.ProcessModeEnum.Always;
			shutdownTimer.OneShot = true;
			shutdownTimer.WaitTime = ServerShutdownDelay;
			shutdownTimer.Timeout += () =>
			{
				if (GetTree().GetMultiplayer().GetPeers().Count() == 0)
				{
					DebugIt("No peers connected. Shutting down server automatically.");
					GetTree().Quit();
				}
				else
				{
					DebugIt("Peers reconnected, shutdown cancelled.");
				}

				shutdownTimer.QueueFree();
				shutdownTimer = null;
			};
			AddChild(shutdownTimer);
			shutdownTimer.Start();

			DebugIt("Shutdown timer started...");
		}

		private void OnPeerDisconnected(long id)
		{
			DebugIt($"Peer disconnected: {id}");

			// host (id == 1) is the server
			if (!_isServer && id == 1)
			{
				GD.Print("Host disconnected. Returning to main menu.");

				// cleanup UI, score, etc.
				var root = GetTree().Root;
				var hud = root.GetNodeOrNull<CanvasLayer>("HUD");
				if (hud != null)
					hud.QueueFree();

				var gameRoot = root.GetNodeOrNull<GameRoot>("GameRoot");
				gameRoot?.CleanupAllLocal();

				GetTree().ChangeSceneToFile("res://Menu/Main/mainMenu.tscn");
				return;
			}

			// if we are the server and a client disconnects, remove the player entity
			if (_isServer && id != 1)
			{
				GD.Print($"Client {id} disconnected. Removing player entity, but game continues.");
				// NO CLEANUP HERE! NO CHANGE SCENE! or it will crash the host game
				return;
			}

			// If all players disconnect from the server, we shut it down
			if (_isServer && GetTree().GetMultiplayer().GetPeers().Count() == 0)
			{
				// kill him!! if he is a lonely server, lost in the sad world of the web with no one to play with ;(
				DebugIt("No peers connected due to disconnects. Shutting down server.");
				GetTree().Quit();
			}
		}

		private bool IsPortOpen(string host, int port, int timeout = 500)
		{
			try
			{
				using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				var result = socket.BeginConnect(host, port, null, null);
				bool success = result.AsyncWaitHandle.WaitOne(timeout, true);
				if (!success)
					return false;

				// erst hier das Ende des Connect-Versuchs abwarten
				socket.EndConnect(result);
				return socket.Connected;
			}
			catch
			{
				return false;
			}
		}

		public async Task WaitForServerAsync()
		{
			var timeout = Stopwatch.StartNew();
			while (!IsPortOpen(GetServerIPAddress(), RPC_PORT, 500))
			{
				if (timeout.Elapsed.TotalSeconds > 10)
				{
					GD.PrintErr("Timeout: Server unreachable after 10 seconds.");
					throw new TimeoutException();
				}
				// pause to avoid maxing out CPU
				await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
			}

			DebugIt("Connection established successfully!");
		}

		private void HandleServerUdp()
		{
			// poll for new connections
			_udpServer.Poll();
			AcceptNewUdpConnections();
			// receive incoming commands
			ReceiveServerCommands();

			if (enableDebug)
				GD.Print($"[SERVER][UDP] Peers: {_udpPeers.Count}, Tick: {_tick}");
		}

		private void AcceptNewUdpConnections()
		{
			while (_udpServer.IsConnectionAvailable())
			{
				var peer = _udpServer.TakeConnection();
				if (peer != null)
				{
					// discard initial handshake
					if (peer.GetAvailablePacketCount() > 0)
						peer.GetPacket();
					_udpPeers.Add(peer);
				}
			}
		}

		private void ReceiveServerCommands()
		{
			foreach (var peer in _udpPeers)
			{
				while (peer.GetAvailablePacketCount() > 0)
				{
					var data = peer.GetPacket();
					var text = Encoding.UTF8.GetString(data);
					if (text == "HELLO")
					{
						DebugIt($"Handshake von {peer.GetPacketIP()}:{peer.GetPacketPort()} erhalten");
						continue;
					}

					// deserialize command
					var cmd = Serializer.Deserialize<Command>(data);
					_incomingCommands.Enqueue(cmd);
				}
			}
		}

		private void HandleClientUdp()
		{
			while (_udpClientPeer.GetAvailablePacketCount() > 0)
			{
				var data = _udpClientPeer.GetPacket();
				var text = Encoding.UTF8.GetString(data);

				if (text == "START")
				{
					DebugIt("Received START packet → switching to Game scene");
					CallDeferred(nameof(NotifyGameStart));
					continue;
				}

				var snap = Serializer.Deserialize<Snapshot>(data);
				client.ApplySnapshot(snap);
				DebugIt($"Received snapshot tick={snap.Tick}, entities={snap.Entities.Count}");
				if (enableDebug) {
                    GD.Print($"[CLIENT][UDP] Received snapshot tick={snap.Tick}, entities={snap.Entities.Count}");
				}
			}
		}

		private void HandleTickLoop(double delta)
		{
			_acc += delta;
			while (_acc >= TICK_DELTA)
			{
				_acc -= TICK_DELTA;
				if (_isServer)
				{
					if (enableDebug)
					{
						GD.Print($"[SERVER][TICK] Tick={_tick}, Commands={_incomingCommands.Count}");
					}
					ProcessServerTick();
				}
				else
				{
					if (enableDebug)
					{
						GD.Print($"[CLIENT][TICK] Tick={_tick}");
					}
					SendClientCommand();
				}
				_tick++;
			}
		}


		[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
		public void NotifyGameStart()
		{
			SoloMode = false;
			// change scene to game
			var gameScene = GD.Load<PackedScene>("res://Utilities/GameRoot/GameRoot.tscn");
			gameScene.Instantiate<Node>();
			GetTree().ChangeSceneToPacked(gameScene);
			var peerId = GetTree().GetMultiplayer().GetUniqueId();
			GD.PrintErr($"Game started by peer: {peerId}");
			DebugIt($"NotifyGameStart called by (PeerID: {peerId})");

			_gameRunning = true;
		}

		[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
		public void SelectCharacter(int selectedCharacterId, int health, int speed, int dexterity, int strength, int intelligence)
		{
			long peerId = Multiplayer.GetRemoteSenderId();
//			Server.Instance.PlayerSelections[peerId] = new PlayerCharacterData { CharacterId = selectedCharacterId };
			DebugIt("Player selectged: " + selectedCharacterId + " By PlayerID: " + peerId);
			GD.PrintErr($"Game peer {peerId} selected character {selectedCharacterId}");

			// Speichere alle Werte pro Spieler
			Server.Instance.PlayerSelections[peerId] = new PlayerCharacterData
			{
				CharacterId = selectedCharacterId,
				Health = health,
				Speed = speed,
				Dexterity = dexterity,
				Strength = strength,
				Intelligence = intelligence
			};
			DebugIt($"Player {peerId} selected character {selectedCharacterId} (HP: {health}, Speed: {speed}, Dex: {dexterity}, Str: {strength}, Int: {intelligence} ...)");
		}

		// maybe reactivate for online multiplayer
		/* 		public void NotifyGameStartUDP()
				{
					var startPacket = Encoding.UTF8.GetBytes("START");
					foreach (var peer in _udpPeers)
						peer.PutPacket(startPacket);

					CallDeferred(nameof(NotifyGameStart));
				} */
		private void ProcessServerTick()
		{
			// apply all commands
			while (_incomingCommands.Count > 0)
			{
				var cmd = _incomingCommands.Dequeue();
				Server.Instance.ProcessCommand(cmd);
			}

			// send snapshot to all clients
			foreach (var peer in _udpPeers)
			{
				peer.PutPacket(Server.Instance.GetSnapshot(_tick));
			}
		}

		private void SendClientCommand()
		{
			if (_udpClientPeer == null) return;

			Command cmdShop = client.GetShopCommand(_tick);

			if (cmdShop != null)
			{
				_udpClientPeer.PutPacket(Serializer.Serialize(cmdShop));
				DebugIt($"Send Shop cmd tick={_tick}, dir={cmdShop.Weapon}");
			}

			Command cmd = client.GetCommand(_tick);

			if (cmd == null) return;

			_udpClientPeer.PutPacket(Serializer.Serialize(cmd));
			DebugIt($"Send MOVE cmd tick={_tick}, dir={cmd.MoveDir}");
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

		public string GenerateConnectionCode()
		{
			string ip = GetServerIPAddress();
			var parts = ip.Split('.');
			if (parts.Length != 4)
				return "ERR";
			return $"{parts[3]}";
		}

		private string ResolveConnectionCode(string code)
		{
			string localIp = GetServerIPAddress();
			var prefix = string.Join(".", localIp.Split('.').Take(3));
			return $"{prefix}.{code}";
		}

		private void DebugIt(string message)
		{
			if (enableDebug) Debug.Print("Network Manager: " + message);
		}

		public void CleanupNetworkState()
		{
			_isServer = false;
			_gameRunning = false;
			_readyForUdp = false;
			_udpPeers.Clear();

			foreach (var child in GetChildren().ToList())
			{
				if (child is Server || child is Client)
					RemoveChild(child);
				if (child is Node node)
					node.QueueFree();
			}

			server = null;
			client = null;

			var multiplayer = GetTree().GetMultiplayer();
			if (multiplayer.MultiplayerPeer != null)
			{
				multiplayer.MultiplayerPeer.Close();
				multiplayer.MultiplayerPeer = null;
				DebugIt("MultiplayerPeer set to null (Cleanup).");
			}

			if (_udpClientPeer != null)
			{
				_udpClientPeer.Close();
				_udpClientPeer = null;
				DebugIt("UDP Client disconnected (Cleanup).");
			}

			if (_udpServer != null)
			{
				_udpServer = null;
				DebugIt("UDP Server disconnected (Cleanup).");
			}

			if (_rpcClientPeer != null)
			{
				_rpcClientPeer.Close();
				_rpcClientPeer.Dispose();
				_rpcClientPeer = null;
				DebugIt("ENetMultiplayerPeer (Client) disconnected (Cleanup).");
			}

			if (_rpcServerPeer != null)
			{
				_rpcServerPeer.Close();
				_rpcServerPeer.Dispose();
				_rpcServerPeer = null;
				DebugIt("ENetMultiplayerPeer (Server) disconnected (Cleanup).");
			}

			GC.Collect();
		}
	}
}
