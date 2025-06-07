using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
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

			var alivePlayers = new List<DefaultPlayer>();
			foreach (Node node in GetTree().GetNodesInGroup("player"))
			{
				GD.Print($"[WaveTimer] Found node in 'player' group: {node.Name} ({node.GetType().Name})");
				if (node is DefaultPlayer player)
					GD.Print($"[WaveTimer]   -> alive: {player.alive}, OwnerPeerId: {player.OwnerPeerId}");
				if (node is DefaultPlayer player2 && player2.alive)
					alivePlayers.Add(player2);
			}
			GD.Print($"[WaveTimer] Total alivePlayers: {alivePlayers.Count}");
			int bonus = 100 * (WaveCounter - 1); // Fixed bonus per player, not multiplied by survivors

			foreach (var player in alivePlayers)
			{
				long playerId = player.OwnerPeerId;
				GD.Print($"[WaveTimer] AddBonus for playerId={playerId}, bonus={bonus}");
				Game.Utilities.Backend.ScoreManager.AddBonus(playerId, bonus);

				var floatingScoreScene = GD.Load<PackedScene>("res://UI/FloatingScore/floating_score.tscn");
				var floatingScore = floatingScoreScene.Instantiate<FloatingScore>();
				floatingScore.Text = $"+{bonus} Wave Bonus!";

				
				floatingScore.SetPlayerColorById(playerId);

				GetTree().Root.AddChild(floatingScore);
				floatingScore.GlobalPosition = player.GlobalPosition;
			}
		}

		_waveCounterLabel.Text = $"Wave: {WaveCounter}";
		_timeLeftLabel.Text = _waveTimer.Paused ? "Grace Time" : (MaxTime - SecondCounter).ToString();
		IsPaused = _waveTimer.Paused;
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
