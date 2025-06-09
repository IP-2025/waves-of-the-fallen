using Godot;
using Game.Utilities.Multiplayer;

public partial class PauseMenu : Control
{
	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always; // important to continue while paused
		GetNode<Button>("Background/VBoxContainer/Resume").Pressed += OnResumePressed;
		GetNode<Button>("Background/VBoxContainer/Settings").Pressed += OnSettingsPressed;
		GetNode<Button>("Background/VBoxContainer/Main Menu").Pressed += OnMainMenuPressed;
		GetNode<Button>("Background/VBoxContainer/Quit").Pressed += OnQuitPressed;
		Visible = false;
	}

	private void OnResumePressed()
	{
		Visible = false;
	
		if (NetworkManager.Instance._soloMode)
			GetTree().Paused = false;
	}

	private void OnSettingsPressed()
	{
		var settingsScene = GD.Load<PackedScene>("res://Menu/Settings/settingsMenu.tscn");
		var settingsMenu = settingsScene.Instantiate();
		GetTree().Root.AddChild(settingsMenu);
		Visible = false;
	}

	private void OnMainMenuPressed()
	{
		var dialogScene = GD.Load<PackedScene>("res://Menu/PauseMenu/leaveWarningDialog.tscn");
		var dialog = dialogScene.Instantiate<ConfirmationDialog>();
		dialog.ProcessMode = ProcessModeEnum.Always; 
		dialog.DialogText = "Are you sure you want to leave the game?\nIn multiplayer you will lose gold as a penalty!";
		dialog.GetOkButton().Text = "Yes";
		dialog.GetCancelButton().Text = "No";
		dialog.Confirmed += OnLeaveConfirmed;
		GetTree().Root.AddChild(dialog);
		dialog.PopupCentered();
	}

	private void OnLeaveConfirmed()
	{
		if (!NetworkManager.Instance._soloMode)
		{
			// send leave message to server
			RpcId(1, "PlayerLeft", Multiplayer.GetUniqueId());

			// TODO: Backend or local penalty for leaving multiplayer
			GD.Print("Penalty: Player loses gold for leaving multiplayer!");
		}
		// cleanup local game state
		var gameRoot = GetTree().Root.GetNodeOrNull<GameRoot>("GameRoot");
		gameRoot?.CleanupAllLocal();
		NetworkManager.Instance.CleanupNetworkState();

		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://Menu/Main/mainMenu.tscn");
	}

	private void OnQuitPressed()
	{
		GetTree().Quit();
	}

	public override void _Input(InputEvent @event)
	{
		if (Visible && @event.IsActionPressed("ui_cancel"))
		{
			Visible = false;
			if (NetworkManager.Instance._soloMode)
				GetTree().Paused = false;
			// waste input event to prevent it from propagating further
			@event.Dispose();
		}
	}
}
