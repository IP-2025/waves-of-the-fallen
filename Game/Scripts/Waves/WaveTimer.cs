using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.WebSockets;
using Godot;

public partial class WaveTimer : Node2D
{
	public int waveCounter = 1;
	public int secondCounter = 0;
	public int maxTime = 5;
	private Timer waveTimer;

	public override void _Ready()
	{

		foreach (var player in GameManager.Players)
		{ // is for making the UI for the timer and wave counter only visible for one player and not for all players in the game
			if (player.Id == Multiplayer.GetUniqueId())
			{
				GetNode<Label>("/root/Node2D/" + player.Id + "/Camera2D/TimeLeft").Text = maxTime.ToString();
				GetNode<Label>("/root/Node2D/" + player.Id + "/Camera2D/WaveCounter").Text = "Wave: " + waveCounter;
			}
		}
		waveTimer = GetNode<Timer>("WaveTimer");
		waveTimer.Timeout += OnTimerTimeout; // calls method every second
	}


	private void OnTimerTimeout()
	{
		secondCounter++; // counts the seconds until the max_time is reached and a new wave begins

		if (secondCounter >= maxTime)
		{
			secondCounter = 0;
			waveCounter++;
			
			GetNode<Label>("/root/Node2D/" + Multiplayer.GetUniqueId() + "/Camera2D/WaveCounter").Text = "Wave: " + waveCounter;
			DeleteEnemies();
		}
		Label label = GetNode<Label>("/root/Node2D/" + Multiplayer.GetUniqueId() + "/Camera2D/TimeLeft");
		var zeit = maxTime - secondCounter;
		label.Text = zeit.ToString(); // changes text to the remaining time until the next wave
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
			Debug.Print("No enemies found");
		}
	}

	public void PauseUnpauseTimer() // Flips the paused state of waveTimer
	{
		if (Multiplayer.IsServer())
		{
			if (waveTimer.Paused) Debug.Print("WaveTimer unpaused");
			else Debug.Print("WaveTimer paused");
			waveTimer.Paused = !waveTimer.Paused;
		}
	}
}
