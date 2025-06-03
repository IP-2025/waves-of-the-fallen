using Godot;
using Game.Utilities.Multiplayer;

namespace Game.UI.GameOver{
	public partial class GameOverScreen : Control
{
	[Export] public Label ScoreLabel;
	[Export] public Button MainMenuBtn;
	[Export] public ColorRect FadeRect;
	[Export] public Label GameOverLabel;
	[Export] public AudioStreamPlayer AudioPlayer;
	[Export] public AnimationPlayer AnimationPlayerBackground;
	[Export] public AnimationPlayer AnimationPlayerForeground;

	public override void _Ready()
		{
			// Initial Setup
			MainMenuBtn.Visible = false;

			AnimationPlayerBackground.Play("FadeIn");
			AnimationPlayerForeground.Play("GameOver");
			MainMenuBtn.Visible = true;

			MainMenuBtn.Pressed += OnMainMenuBtnPressed;
		}

	public void SetScore(int score)
	{
		ScoreLabel.Text = $"Score: {score}";
	}

	private void OnMainMenuBtnPressed()
	{
		GetTree().ChangeSceneToFile("res://Menu/Main/MainMenu.tscn");
	}
}
}
