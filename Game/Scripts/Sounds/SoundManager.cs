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

		player.Position = position;															//Gibt die Position weiter, um von dort aus ein Sound abzuspielen
		player.Stream = GD.Load<AudioStream>("res://Assets/Sounds/" + name + ".wav");		//Nimmt von dem Sounds Ordner den richtigen Sound
		player.VolumeDb = volumeDb;															//(Optional) Stellt die Lautstärke bei Verlangen um

		player.Play();																		//Spielt den Sound

		player.Finished += player.QueueFree;												//Löscht den Player nachdem der Sound fertig abgespielt ist
	}

	public void PlayUI(string name, float volumeDb = 0.0f)
	{
		AudioStreamPlayer player = new AudioStreamPlayer();
		AddChild(player);

		player.Stream = GD.Load<AudioStream>("res://Assets/Sounds/" + name + ".wav");
		player.VolumeDb = volumeDb;

		player.Play();

		player.Finished += player.QueueFree;
	}

}
