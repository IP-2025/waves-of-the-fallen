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
		foreach (var player in GameManager.Players) { // is for making the UI for the timer and wave counter only visible for one player and not for all players in the game
			if (player.Id != Multiplayer.GetUniqueId()) {
				GetNode<Label>("/root/Node2D/" + player.Id + "/Camera2D/TimeLeft").QueueFree();
				GetNode<Label>("/root/Node2D/" + player.Id + "/Camera2D/WaveCounter").QueueFree();
			}
		}
		Timer wave_timer = GetNode<Timer>("WaveTimer"); 
		wave_timer.Timeout += OnTimerTimeout; // calls method every second
	}


	private void OnTimerTimeout()
	{
		second_counter++; // counts the seconds until the max_time is reached and a new wave begins
		if (second_counter >= max_time)
		{
			IncreaseWaveCounter(); // increases wave counter when the last wave is cleared 
			if (GetTree().GetNodesInGroup("Enemies") != null)
			{
				Debug.Print("Starting enemy deletion");
				foreach (Node2D enemy in GetTree().GetNodesInGroup("Enemies")) // deletes every enemy
				{
					//Debug.Print("Deleted enemy");
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
		label.Text = zeit.ToString(); // changes text to the remaining time until the next wave
	}

	private void IncreaseWaveCounter()
	{
		wave_counter++;
		GetNode<Label>("/root/Node2D/" + Multiplayer.GetUniqueId() + "/Camera2D/WaveCounter").Text = "Wave: " + wave_counter;
	}
}
