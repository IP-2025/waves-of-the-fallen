using Godot;
using System;

public partial class SoundManager : Node2D
{
	public static SoundManager Instance;

	//private AudioStreamPlayer2D _sfxPlayer;
	private AudioStreamPlayer2D _musicPlayer;
	private AudioStreamPlayer2D _buttonPlayer;

	public override void _Ready()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		_musicPlayer = GetNode<AudioStreamPlayer2D>("MusicPlayer");
		_buttonPlayer = GetNode<AudioStreamPlayer2D>("ButtonSound");
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

	public void PlayUI(AudioStream stream)
	{
		_buttonPlayer.Stream = stream;
		_buttonPlayer.Play();
	}

	public void PlayMusic(AudioStream stream)
	{
		_musicPlayer.Stream = stream;
		_musicPlayer.Autoplay = true;
		_musicPlayer.Play();
	}

}
