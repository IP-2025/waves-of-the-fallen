using System.Diagnostics;
using System.Threading.Tasks;
using Godot;

public partial class WaveTimer : Node2D
{
	public bool disable	= false;
	public int waveCounter = 1;
	public int secondCounter = 0;
	public int maxTime = 30;
	public bool isPaused;
	private Timer _waveTimer;
	private Label _timeLeftLabel;
	private Label _waveCounterLabel;

	[Signal]
	public delegate void WaveEndedEventHandler();

	public override void _Ready()
	{
		_timeLeftLabel = GetNode<Label>("TimeLeft");
		_waveCounterLabel = GetNode<Label>("WaveCounter");

		// set values
		_timeLeftLabel.Text = maxTime.ToString();
		_waveCounterLabel.Text = $"Wave: {waveCounter}";

		// get timer and callback
		_waveTimer = GetNode<Timer>("WaveTimer");
		_waveTimer.Timeout += OnTimerTimeout;
	}


	private void OnTimerTimeout()
	{
		if (disable) return;
		
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
		if (_waveTimer.Paused) 
		{
			_timeLeftLabel.Text = "Grace Time";
			isPaused = true;
		} else{
			isPaused = false;
		}
	}

	public async Task PauseTimer(int time) // Flips the paused state of waveTimer
	{
		_waveTimer.Paused = true;
		Debug.Print("WaveTimer paused");
		await ToSignal(GetTree().CreateTimer(time), SceneTreeTimer.SignalName.Timeout);

		_waveTimer.Paused = false;
		Debug.Print("WaveTimer unpaused");
	}
}
