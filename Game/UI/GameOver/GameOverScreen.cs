using Godot;
using System;

namespace Game.UI.GameOver
{
	public partial class GameOverScreen : Control
	{
		[Export] private Label _scoreLabel;
		[Export] private Button _mainMenuButton;
		[Export] private AnimationPlayer _animationPlayer;

		public override void _Ready()
		{
			_mainMenuButton.Visible = false;
			_mainMenuButton.Pressed += OnMainMenuButtonPressed;
		}

		public void ShowGameOver(int score)
		{
			_scoreLabel.Text = $"Score: {score}";
			_animationPlayer.Play("FadeIn");
			_mainMenuButton.Visible = true;
		}

		private void OnMainMenuButtonPressed()
		{
			GetTree().ChangeSceneToFile("res://Game/UI/MainMenu.tscn");
		}
	}
}
