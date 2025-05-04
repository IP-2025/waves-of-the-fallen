using System.Diagnostics;
using Godot;

public partial class WaveTimer : Node2D
{
	public int waveCounter = 1;
	public int secondCounter = 0;
	public int maxTime = 30;
	private Timer _waveTimer;
	private Label _timeLeftLabel;
	private Label _waveCounterLabel;

	[Signal]
	public delegate void WaveEndedEventHandler();

	public override void _Ready()
	{
		// SORRY hat to change this because of the multiplayer stuff

		// Find local player with entity map
		var localId = Multiplayer.GetUniqueId();
		if (!GameManager.Instance.Entities.TryGetValue(localId, out var playerNode))
		{
			GD.PrintErr($"WaveTimer: Cant find PlayerNode for ID {localId}");
			return;
		}

		// Get cam from player node
		var cam = playerNode.GetNode<Camera2D>("Camera2D");
		_timeLeftLabel = cam.GetNode<Label>("TimeLeft");
		_waveCounterLabel = cam.GetNode<Label>("WaveCounter");

		// set values
		_timeLeftLabel.Text = maxTime.ToString();
		_waveCounterLabel.Text = $"Wave: {waveCounter}";

		// get timer and callback
		_waveTimer = GetNode<Timer>("WaveTimer");
		_waveTimer.Timeout += OnTimerTimeout;
	}


	private void OnTimerTimeout()
	{
		secondCounter++; // counts the seconds until the max_time is reached and a new wave begins
		if (secondCounter >= maxTime)
		{
			secondCounter = 0;
			waveCounter++;

			_waveCounterLabel.Text = $"Wave: {waveCounter}";
			EmitSignal(SignalName.WaveEnded);
		}

		_waveCounterLabel.Text = $"Wave: {waveCounter}";
		_timeLeftLabel.Text = (maxTime - secondCounter).ToString();
	}

	public void PauseUnpauseTimer() // Flips the paused state of waveTimer
	{
		_waveTimer.Paused = !_waveTimer.Paused;

		if (_waveTimer.Paused)
		{
			Debug.Print("WaveTimer paused");
		}
		else
		{
			Debug.Print("WaveTimer unpaused");
		}

	}
}
