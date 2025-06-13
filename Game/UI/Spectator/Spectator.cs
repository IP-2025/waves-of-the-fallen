using Godot;
using System;
using System.Linq;

public partial class Spectator : CanvasLayer
{
	// Reference to the background overlay that darkens the screen
	private ColorRect overlay;

	// Reference to the label showing the name of the player being spectated
	private Label nameLabel;
	
	// References for the buttons
	private Button leftButton;
	private Button rightButton;

	// Index of the currently spectated player
	private int currentIndex = 0;

	// List of all alive players that can be spectated
	private Godot.Collections.Array<Node> alivePlayers;

	public override void _Ready()
	{
		// Get references to UI elements
		overlay = GetNode<ColorRect>("Overlay");
		nameLabel = GetNode<Label>("ButtonContainer/PlayerName");
		leftButton = GetNode<Button>("ButtonContainer/LeftButton");
		rightButton = GetNode<Button>("ButtonContainer/RightButton");
		
		// Connect the button signals to the script methods 
		if (leftButton != null)
		{
			leftButton.Pressed += _on_LeftButton_pressed;
		}
		if (rightButton != null)
		{
			rightButton.Pressed += _on_RightButton_pressed;
		}

		// Hide the entire spectator UI initially
		Visible = false;
		overlay.Visible = false;
		nameLabel.Visible = false;
		
		GD.Print($"Spectator added. Parent: {GetParent()?.Name}");

		// Initialize alivePlayers list
		alivePlayers = GetTree().GetNodesInGroup("player");

		// Start by showing the first valid target if available
		UpdateSpectatedPlayer();
	}

	// This method toggles the visibility of the spectator UI
	public void EnableUI(bool enabled)
	{
		Visible = enabled;

		// Also show/hide individual parts of the UI
		if (overlay != null) overlay.Visible = enabled;
		if (nameLabel != null) nameLabel.Visible = enabled;
		
		// Also toggle visibility for the buttons
		if (leftButton != null) leftButton.Visible = enabled;
		if (rightButton != null) rightButton.Visible = enabled;

		if (enabled)
		{
			// Refresh the list of alive players when activating UI
			alivePlayers = GetTree().GetNodesInGroup("player");

			// Reset to the first player
			currentIndex = 0;
			UpdateSpectatedPlayer();
		}
	}

	// Sets the name of the currently spectated player in the UI
	public void SetPlayerName(string name)
	{
		if (nameLabel != null)
			nameLabel.Text = name;
	}

	// Called when the "<" button is pressed – switch to previous player
	public void _on_LeftButton_pressed()
	{
		if (alivePlayers.Count == 0) return;

		currentIndex = (currentIndex - 1 + alivePlayers.Count) % alivePlayers.Count;
		UpdateSpectatedPlayer();
		GetNode<GameRoot>("/root/GameRoot").OnSpectatorSwitchTarget(-1);
	}

	// Called when the ">" button is pressed – switch to next player
	public void _on_RightButton_pressed()
	{
		if (alivePlayers.Count == 0) return;

		currentIndex = (currentIndex + 1) % alivePlayers.Count;
		UpdateSpectatedPlayer();
		GetNode<GameRoot>("/root/GameRoot").OnSpectatorSwitchTarget(1);
	}

	// Updates the currently spectated player's name and sets camera focus
	private void UpdateSpectatedPlayer()
	{
		if (alivePlayers.Count == 0)
			return;

		// Clamp index if players died
		currentIndex = Mathf.Clamp(currentIndex, 0, alivePlayers.Count - 1);

		var player = alivePlayers[currentIndex] as Node2D;
		if (player == null)
			return;

		// Set camera of this player as current
		var camera = player.GetNodeOrNull<Camera2D>("Camera2D");
		if (camera != null)
		{
			camera.MakeCurrent();
		}

		// Update name label
		SetPlayerName(player.Name);
	}

}
