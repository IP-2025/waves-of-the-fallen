using System.Diagnostics;
using Godot;

public partial class WaveTimer : Node2D
{
	public bool disable	= false;
	public int waveCounter = 1;
	public int secondCounter = 0;
	public int maxTime = 30;
	private Timer _waveTimer;
	private Label _timeLeftLabel;
	private Label _waveCounterLabel;

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
			DeleteEnemies();
		}

		_waveCounterLabel.Text = $"Wave: {waveCounter}";
		_timeLeftLabel.Text = (maxTime - secondCounter).ToString();
	}

	private void DeleteEnemies()
	{
		if (GetTree().GetNodesInGroup("Enemies") != null)
		{
			Debug.Print("Deleting " + GetTree().GetNodeCountInGroup("Enemies") + " enemies");
			foreach (Node2D enemy in GetTree().GetNodesInGroup("Enemies")) // deletes every enemy
			{
				//Debug.Print("Deleted enemy");
				enemy.QueueFree();
			}
		}
		else
		{
			Debug.Print("No enemies found to be deleted");
		}
	}
}
