using Godot;
using System.Threading.Tasks;

namespace Game.UI.GameOver{
	public partial class GameOverScreen : Control
{
	[Export] public Label ScoreLabel;
	[Export] public Button MainMenuBtn;
	[Export] public ColorRect FadeRect;
	[Export] public HBoxContainer GameOverBox;
	[Export] public Label GameLabel;
	[Export] public Label OverLabel;
	[Export] public AudioStreamPlayer AudioPlayer;

	public override void _Ready()
	{
		// Initial Setup
		MainMenuBtn.Visible = false;
		GameOverBox.Visible = true;

		// Labels außerhalb des Sichtbereichs platzieren (Animation-Start)
		GameLabel.Position = new Vector2(-400, 0);
		OverLabel.Position = new Vector2(600, 0);

		// Animation starten
		AnimateGameOver();
		FadeToGrey();
		PlaySound();
		ShowButtonDelayed();

		// Button Signal verbinden
		MainMenuBtn.Pressed += OnMainMenuBtnPressed;
	}

	public void SetScore(int score)
	{
		ScoreLabel.Text = $"Score: {score}";
	}

	private void AnimateGameOver()
	{
		// GameLabel von links zur Mitte
		var tweenGame = GetNode<Tween>("GameOverBox/TweenGame");
		tweenGame.TweenProperty(GameLabel, "position:x", 0, 1.0f);
		tweenGame.Play();

		// OverLabel von rechts zur Mitte
		var tweenOver = GetNode<Tween>("GameOverBox/TweenOver");
		tweenOver.TweenProperty(OverLabel, "position:x", 0, 1.0f);
		tweenOver.Play();
	}

	private void FadeToGrey()
	{
		var tween = CreateTween();
		tween.TweenProperty(FadeRect, "modulate:a", 0.7f, 2.0f);
		tween.Play();
	}

	private void PlaySound()
	{
		AudioPlayer.Play();
	}

	private async void ShowButtonDelayed()
	{
		await ToSignal(GetTree().CreateTimer(2), "timeout");
		MainMenuBtn.Visible = true;
	}

	private void OnMainMenuBtnPressed()
	{
		GetTree().ChangeSceneToFile("res://Menu/Main/MainMenu.tscn");
	}
}
}
