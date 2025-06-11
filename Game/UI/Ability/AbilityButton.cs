using Godot;
using Godot.NativeInterop;
using System;
using System.Diagnostics;

public partial class AbilityButton : Node2D
{
	public bool Disable { get; set; }
	public int SecondCounter { get; private set; } = 0;
	public int MaxTime { get; private set; }
	public bool IsPaused { get; private set; }
	private Node2D _parent;
	private Timer _abilityTimer;
	private Sprite2D _abilityPic;
	private Label _timeLeftLabel;
	private CharacterManager _characterManager;
	private AbilityBase abilityBase;
	private PackedScene packedScene;
	public override void _Ready()
	{
		_timeLeftLabel = GetNode<Label>("TimeLeft");
		_abilityPic = GetNode<Sprite2D>("%AbilityPic");
		_parent = GetParent<Node2D>();
		_characterManager = GetNode<CharacterManager>("/root/CharacterManager");
		var selectedId = _characterManager.LoadLastSelectedCharacterID();

		/*if (_parent is DefaultPlayer player)
		{
			packedScene = ((DefaultPlayer) _parent)._abilityScene;
			Debug.Print(packedScene.Instantiate().ToString());
			abilityBase = (AbilityBase)packedScene.Instantiate().GetScript();
			MaxTime = abilityBase.getCooldown();
			Debug.Print(MaxTime.ToString());
		}*/



		_timeLeftLabel.Text = MaxTime.ToString();
		_abilityTimer = GetNode<Timer>("AbilityTimer");
		_abilityTimer.Timeout += OnTimerTimeout;
		SecondCounter = MaxTime;
		Disable = false;
	}

	private void OnTimerTimeout()
	{
		SecondCounter++; // counts the seconds until the max_time is reached and the ability is ready to use again
		if (SecondCounter >= MaxTime)
		{
			SecondCounter = 0;
			_abilityTimer.Stop();
			_timeLeftLabel.Hide();
			Disable = false;
			//Pic for ability ready show
		}
		_timeLeftLabel.Text = (MaxTime - SecondCounter).ToString();
		IsPaused = _abilityTimer.Paused;
	}

	public void _on_ability_button_button_down()
	{
		if (!Disable)
		{
			//call to the chars ability here
			if (_parent is DefaultPlayer player)
			{
				Disable = true;
				player.UseAbility();
			}
			_abilityTimer.Start();
			_timeLeftLabel.Show();
		}
	}
}
