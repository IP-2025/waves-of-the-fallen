using System.Diagnostics;
using System.Threading.Tasks;
using Godot;

public partial class WaveTimer : Node2D
{
	public bool Disable { get; set; } = false;
	public int WaveCounter { get; private set; } = 1;
	public int SecondCounter { get; private set; } = 0;
	public int MaxTime { get; private set; } = 30;
	public bool IsPaused { get; private set; }
	private Timer _waveTimer;
	private Label _timeLeftLabel;
	private Label _waveCounterLabel;

	[Signal]
	public delegate void WaveEndedEventHandler();

	[Signal]
	public delegate void WaveStartedEventHandler();

	public override void _Ready()
	{
		_timeLeftLabel = GetNode<Label>("TimeLeft");
		_waveCounterLabel = GetNode<Label>("WaveCounter");

		// set values
		_timeLeftLabel.Text = MaxTime.ToString();
		_waveCounterLabel.Text = $"Wave: {WaveCounter}";

		// get timer and callback
		_waveTimer = GetNode<Timer>("WaveTimer");
		_waveTimer.Timeout += OnTimerTimeout;
	}

	public void TriggerWaveEnded()
	{
		EmitSignal(nameof(WaveEnded));
	}



	private void OnTimerTimeout()
	{
		if (Disable) return;

		SecondCounter++; // counts the seconds until the max_time is reached and a new wave begins
		if (SecondCounter >= MaxTime)
		{
			SecondCounter = 0;
			WaveCounter++;

			_waveCounterLabel.Text = $"Wave: {WaveCounter}";
			EmitSignal(SignalName.WaveEnded);
		}

		_waveCounterLabel.Text = $"Wave: {WaveCounter}";
		_timeLeftLabel.Text = _waveTimer.Paused ? "Grace Time" : (MaxTime - SecondCounter).ToString();
		if (SecondCounter < 2 && !_waveTimer.Paused)
		{
			EmitSignal(SignalName.WaveStarted);
		}
	}

	public async Task PauseTimer(int time) // Flips the paused state of waveTimer
	{
		_waveTimer.Paused = true;
		IsPaused = true;
		Debug.Print("WaveTimer paused");
		await ToSignal(GetTree().CreateTimer(time), SceneTreeTimer.SignalName.Timeout);

		_waveTimer.Paused = false;
		IsPaused = false;
		Debug.Print("WaveTimer unpaused");
	}
}
