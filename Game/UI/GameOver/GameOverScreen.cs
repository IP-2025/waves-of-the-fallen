using Godot;
using Game.Utilities.Multiplayer;

namespace Game.UI.GameOver{
	public partial class GameOverScreen : CanvasLayer
{
	[Export] public Label ScoreLabel;
	[Export] public Button MainMenuBtn;
	[Export] public ColorRect FadeRect;
	[Export] public Label GameOverLabel;
	[Export] public AnimationPlayer AnimationPlayerBackground;
	[Export] public AnimationPlayer AnimationPlayerForeground;

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
		}

	public void SetScore(int score)
	{
		ScoreLabel.Text = $"Score: {score}";
	}

	private void OnMainMenuBtnPressed()
{
	EmitSignal(nameof(QuitPressed));
	GetTree().ChangeSceneToFile("res://Menu/Main/mainMenu.tscn");
}
}
}
