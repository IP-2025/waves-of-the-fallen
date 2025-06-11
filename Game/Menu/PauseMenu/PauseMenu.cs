using Godot;
using Game.Utilities.Multiplayer;
using Game.Utilities.Backend;

public partial class PauseMenu : Control
{
	public override void _Ready()
	{
		ScoreManager.Reset();
		GameState.CurrentState = ConnectionState.Online; 
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
		var settingsScene = GD.Load<PackedScene>("res://Menu/Settings/inGameSettingsMenu.tscn");
		var settingsMenu = settingsScene.Instantiate();
		settingsMenu.Name = "InGameSettingsMenu";
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
		
		GetTree().Paused = false;

		ScoreManager.Reset();
		GameState.CurrentState = ConnectionState.Offline;
		
		if (!NetworkManager.Instance._soloMode)
		{
			RpcId(1, "PlayerLeft", Multiplayer.GetUniqueId());
			GD.Print("Penalty: Player loses gold for leaving multiplayer!");
		}

		// cleanup local game state
		var gameRoot = GetTree().Root.GetNodeOrNull<GameRoot>("GameRoot");
		gameRoot?.CleanupAllLocal();
		NetworkManager.Instance.CleanupNetworkState();

		var hud = GetTree().Root.GetNodeOrNull<CanvasLayer>("HUD");
		if (hud != null)
			hud.QueueFree();

		CallDeferred(nameof(ChangeToMainMenu));
	}

	private void ChangeToMainMenu()
	{
		GetTree().ChangeSceneToFile("res://Menu/Main/mainMenu.tscn");
	}

	private void OnQuitPressed()
	{
		GetTree().Quit();
	}

	public void OpenPauseMenu()
	{
		Visible = true;
		if (NetworkManager.Instance._soloMode)
			GetTree().Paused = true;
		else
			GetTree().Paused = false;
	}

	public override void _Input(InputEvent @event)
	{
		if (Visible && @event.IsActionPressed("ui_cancel"))
		{
			Visible = false;
			if (NetworkManager.Instance._soloMode)
				GetTree().Paused = false;
			else
				GetTree().Paused = false;
			@event.Dispose();
		}
	}

	// not a clean solution: for player joining host to get pause menu	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (!Visible && @event.IsActionPressed("ui_cancel"))
		{
			OpenPauseMenu();
			@event.Dispose();
		}
	}
}
