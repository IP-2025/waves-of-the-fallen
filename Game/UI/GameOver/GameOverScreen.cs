using Godot;
using Game.Utilities.Multiplayer;
using Game.Utilities.Backend;

namespace Game.UI.GameOver
{
	public partial class GameOverScreen : CanvasLayer
	{
		[Export] public Label ScoreLabel;
		[Export] public Button MainMenuBtn;
		[Export] public ColorRect FadeRect;
		[Export] public Label GameOverLabel;
		[Export] public AnimationPlayer AnimationPlayerBackground;
		[Export] public AnimationPlayer AnimationPlayerForeground;
		[Export] public Button RestartButton;

		[Signal]
		public delegate void QuitPressedEventHandler();

		public override void _Ready()
		{
			// Initial Setup
			MainMenuBtn.Visible = false;

			AnimationPlayerBackground.Play("FadeIn");
			AnimationPlayerForeground.Play("GameOver");
			MainMenuBtn.Visible = true;
			SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("gameOver"));

			MainMenuBtn.Pressed += OnMainMenuBtnPressed;
			if (RestartButton != null)
				RestartButton.Pressed += OnRestartBtnPressed;

			if (RestartButton != null)
				RestartButton.Visible = NetworkManager.Instance.SoloMode;
		}

		public void SetScore(int score)
		{
			ScoreLabel.Text = $"Score: {score}";
		}

		private void OnMainMenuBtnPressed()
		{
			GetTree().Paused = false;

			ScoreManager.Reset();

			if (!NetworkManager.Instance.SoloMode)
			{
				NetworkManager.Instance.CleanupNetworkState();

				GetTree().GetMultiplayer().MultiplayerPeer = null;
			}


			CallDeferred(nameof(GoToMainMenu));
		}

		private void GoToMainMenu()
		{
			GetTree().ChangeSceneToFile("res://Menu/Main/mainMenu.tscn");
		}

		private void OnRestartBtnPressed()
		{
			GetTree().ChangeSceneToFile("res://Utilities/GameRoot/GameRoot.tscn");
		}

		// For future implementation: Intended for when the restart button should also work in multiplayer mode.
		/*
		[Rpc(MultiplayerApi.RpcMode.Authority)]
		private void ClientRestart()
		{
			NetworkManager.Instance.CleanupNetworkState();
			GetTree().ChangeSceneToFile("res://Utilities/GameRoot/GameRoot.tscn");
		}

		private void RestartGameForAll()
		{
			GetTree().ChangeSceneToFile("res://Utilities/GameRoot/GameRoot.tscn");
		}
		*/
	}
}
