using System;
using System.Diagnostics;
using System.Net.WebSockets;
using Godot;

public partial class WaveTimer : Node2D
{
	public int wave_counter = 1;
	public int second_counter = 0;
	public int max_time = 30;

	public override void _Ready()
	{
		Timer wave_timer = GetNode<Timer>("WaveTimer");
		wave_timer.Timeout += OnTimerTimeout;
	}


	private void OnTimerTimeout()
	{
		second_counter++;
		if (second_counter >= max_time)
		{
			IncreaseWaveCounter();
			if (GetTree().GetNodesInGroup("Enemies") != null)
			{
				Debug.Print("Starting enemy deletion");
				foreach (Node2D enemy in GetTree().GetNodesInGroup("Enemies"))
				{
					Debug.Print("Deleted enemy");
					enemy.QueueFree();
				}
				second_counter = 0;
			}
			else
			{
				Debug.Print("No enemies found");
			}
		}
		Label label = GetNode<Label>("/root/Node2D/" + Multiplayer.GetUniqueId() + "/Camera2D/TimeLeft");
		var zeit = max_time - second_counter;
		label.Text = zeit.ToString();
	}

	private void IncreaseWaveCounter()
	{
		wave_counter++;
		Debug.Print(GetTree().Root.GetTreeStringPretty());
		GetNode<Label>("/root/Node2D/" + Multiplayer.GetUniqueId() + "/Camera2D/WaveCounter").Text = "Wave: " + wave_counter;
	}
}
