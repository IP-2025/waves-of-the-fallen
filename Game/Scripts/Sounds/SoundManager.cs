using Godot;
using System;

public partial class SoundManager : Node2D
{
	public static SoundManager Instance;

	public override void _Ready()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	public void PlaySoundAtPosition(string name, Vector2 position, float volumeDb = 0.0f)
	{
		AudioStreamPlayer2D player = new AudioStreamPlayer2D();
		AddChild(player);

		player.Position = position;															
		player.Stream = GD.Load<AudioStream>("res://Assets/Sounds/" + name + ".wav");		
		player.VolumeDb = volumeDb;															

		player.Play();																		

		player.Finished += player.QueueFree;												
	}

	public void PlaySoundAtPosition(AudioStreamPlayer2D player, Vector2 position, float volumeDb = 0.0f)
	{
		player.Position = position;															
		player.VolumeDb = volumeDb;															

		player.Play();																														
	}

	public void PlayUI(float volumeDb = 0.0f)
	{
		AudioStreamPlayer player = new AudioStreamPlayer();
		AddChild(player);

		player.Stream = GD.Load<AudioStream>("res://Assets/Sounds/buttonPress.wav");
		player.VolumeDb = volumeDb;

		player.Play();

		player.Finished += player.QueueFree;
	}

	public void PlayGameOver(float volumeDb = 0.0f)
	{
		AudioStreamPlayer player = new AudioStreamPlayer();
		AddChild(player);

		player.Stream = GD.Load<AudioStream>("res://Assets/Sounds/gameEnd.wav");
		player.VolumeDb = volumeDb;

		player.Play();

		player.Finished += player.QueueFree;
	}
}
