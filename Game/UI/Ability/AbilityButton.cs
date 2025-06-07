using Godot;
using System;

public partial class AbilityButton : Node2D
{
	public bool Disable { get; set; }
	public int SecondCounter { get; private set; } = 0;
	public int MaxTime { get; private set; } = 3;
	public bool IsPaused { get; private set; }
	private Node2D _parent;
	private Timer _abilityTimer;
	private Label _timeLeftLabel;

	public override void _Ready()
	{
		_timeLeftLabel = GetNode<Label>("TimeLeft");
		_parent = GetParent<Node2D>();
		// set values
		_timeLeftLabel.Text = MaxTime.ToString();

		// get timer and callback
		_abilityTimer = GetNode<Timer>("AbilityTimer");
		_abilityTimer.Timeout += OnTimerTimeout;
		SecondCounter = MaxTime;
		Disable = false;
	}

	private void OnTimerTimeout()
	{
		SecondCounter++; // counts the seconds until the max_time is reached and the ability is ready to use again
		if (SecondCounter >= MaxTime)
		{
			SecondCounter = 0;
			_abilityTimer.Stop();
			_timeLeftLabel.Hide();
			Disable = false;
			//Pic for ability ready show
		}
		_timeLeftLabel.Text = (MaxTime - SecondCounter).ToString();
		IsPaused = _abilityTimer.Paused;
	}

	public void _on_ability_button_button_down()
	{
		if (!Disable)
		{
			_abilityTimer.Start();
			_timeLeftLabel.Show();
			//Pic for ability not ready show
			//call to the chars ability here
			if (_parent is DefaultPlayer player)
			{
				Disable = true;
				player.UseAbility();
			}
		}
	}
}
