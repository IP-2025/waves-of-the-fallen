using Godot;
using System;

public partial class SoundManager : Node2D
{
	public static SoundManager Instance;

	private AudioStreamPlayer2D _sfxPlayer;
	private AudioStreamPlayer2D _musicPlayer;
	private AudioStreamPlayer2D _buttonPlayer;

	public override void _Ready()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		_sfxPlayer = GetNode<AudioStreamPlayer2D>("EnemyHurt");
		_musicPlayer = GetNode<AudioStreamPlayer2D>("MusicPlayer");
		_buttonPlayer = GetNode<AudioStreamPlayer2D>("ButtonSound");
	}

	public void PlaySFX(AudioStream stream)
	{
		_sfxPlayer.Stream = stream;
		_sfxPlayer.Play();
	}

	public void PlaySoundAtPosition(AudioStream stream, Vector2 position, float volumeDb = 0.0f)
	{
		AudioStreamPlayer2D player = new AudioStreamPlayer2D();
		AddChild(player); 

		player.Position = position;
		player.Stream = stream;
		player.VolumeDb = volumeDb;

		player.Play();

		player.Finished += player.QueueFree;
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
