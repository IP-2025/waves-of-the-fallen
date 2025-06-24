using Godot;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Game.Utilities.Multiplayer;
using System.Reflection;
using System.Collections.Generic;
using System;
public partial class MultiplayerCleanup : Node
{
	private bool _enableDebug = false;

	public void StartFullCleanup(MultiplayerApi multiplayer)
	{
		DebugIt("performing complete network cleanup...");

		// cleanup netork manager
		if (NetworkManager.Instance != null)
		{
			NetworkManager.Instance.DisconnectClient();
			NetworkManager.Instance.CleanupNetworkState();
			ForceCleanupNetworkManager();
		}

		CleanupServerInstance();
		CleanupClientInstance();

		// multiplayer peers cleanup
		if (multiplayer.MultiplayerPeer != null)
		{
			multiplayer.MultiplayerPeer.Close();
			multiplayer.MultiplayerPeer = null;
			DebugIt("multiplayerPeer forced to null");
		}

		// garbage collection cleanup
		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();

		DebugIt("complete network cleanup finished");
		
	}

	private void ForceCleanupNetworkManager()
	{
		if (NetworkManager.Instance == null) return;

		var nm = NetworkManager.Instance;

		// reset network manager flags
		nm._isServer = false;
		nm._isLocalHost = false;
		nm.SoloMode = false;

		var nmType = nm.GetType();

		// _gameRunning reset
		var gameRunningField = nmType.GetField("_gameRunning",
			BindingFlags.NonPublic | BindingFlags.Instance);
		gameRunningField?.SetValue(nm, false);

		// _readyForUdp reset  
		var readyForUdpField = nmType.GetField("_readyForUdp",
			BindingFlags.NonPublic | BindingFlags.Instance);
		readyForUdpField?.SetValue(nm, false);

		// _tick reset
		var tickField = nmType.GetField("_tick",
			BindingFlags.NonPublic | BindingFlags.Instance);
		tickField?.SetValue(nm, (ulong)0);

		// _acc reset
		var accField = nmType.GetField("_acc",
			BindingFlags.NonPublic | BindingFlags.Instance);
		accField?.SetValue(nm, 0.0);

		// clear commands
		var incomingCommandsField = nmType.GetField("_incomingCommands",
			BindingFlags.NonPublic | BindingFlags.Instance);
		if (incomingCommandsField?.GetValue(nm) is Queue<Command> commandQueue)
		{
			commandQueue.Clear();
		}

		// remove udp peers
		var udpPeersField = nmType.GetField("_udpPeers",
			BindingFlags.NonPublic | BindingFlags.Instance);
		if (udpPeersField?.GetValue(nm) is List<PacketPeerUdp> udpPeersList)
		{
			foreach (var peer in udpPeersList)
			{
				peer?.Close();
			}
			udpPeersList.Clear();
		}

		DebugIt("NetworkManager force cleanup completed");
	}

	private void CleanupServerInstance()
	{
		// clean server instances
		if (Server.Instance != null)
		{
			DebugIt("Cleaning up Server instance...");

			// clean server entities
			var entitiesToRemove = Server.Instance.Entities.Keys.ToList();
			foreach (var entityId in entitiesToRemove)
			{
				if (Server.Instance.Entities.TryGetValue(entityId, out var entity))
				{
					if (IsInstanceValid(entity))
					{
						entity.QueueFree();
					}
				}
			}
			Server.Instance.Entities.Clear();

			// clean player selections
			Server.Instance.PlayerSelections.Clear();

			// remove server node from tree
			if (IsInstanceValid(Server.Instance))
			{
				Server.Instance.GetParent()?.RemoveChild(Server.Instance);
				Server.Instance.QueueFree();
			}
			// statics cleanup
			if (Server.Instance != null) Server.Instance = new Server();

			DebugIt("Server cleanup completed");
		}
	}

	private void CleanupClientInstance()
	{
		// client cleanup
		var networkManager = NetworkManager.Instance;
		if (networkManager != null)
		{
			// remove all client server child nodes from NetworkManager
			var childrenToRemove = new List<Node>();
			foreach (Node child in networkManager.GetChildren())
			{
				if (child.GetType().Name == "Client" || child.GetType().Name == "Server")
				{
					childrenToRemove.Add(child);
				}
			}

			foreach (var child in childrenToRemove)
			{
				networkManager.RemoveChild(child);
				child.QueueFree();
				DebugIt($"Removed and freed {child.GetType().Name} from NetworkManager");
			}
			DebugIt("Client cleanup completed");
		}
	}

/* 	private void ResetLocalUIState()
	{
		DebugIt("resetting local UI state...");

		// reset button states
		if (LocalMenu._joinButton != null)
		{
			LocalMenu._joinButton.Visible = true;
			LocalMenu._joinButton.Disabled = false;
		}

		if (LocalMenu._hostButton != null)
		{
			LocalMenu._hostButton.Visible = true;
			LocalMenu._hostButton.Disabled = false;
		}

		if (LocalMenu._playButton != null)
		{
			LocalMenu._playButton.Visible = false;
			LocalMenu._playButton.Disabled = true;
		}

		// reset ip input
		if (LocalMenu._ipIo != null)
		{
			LocalMenu._ipIo.Text = "";
		}

		// reset players text
		if (LocalMenu._currentPlayers != null)
		{
			LocalMenu._currentPlayers.Text = "";
			LocalMenu._currentPlayers.SizeFlagsVertical = SizeFlags.ShrinkCenter;
		}

		// reset host flag
		LocalMenu._isHost = false;

		DebugIt("ui state reset completed");
	} */
	private void DebugIt(string message)
	{
		if (_enableDebug) Debug.Print("MultiplayerCleanup " + message);
	}
}
