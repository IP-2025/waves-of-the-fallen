using Godot;
using Godot.NativeInterop;
using System;
using System.ComponentModel;
using System.Diagnostics;

public partial class AbilityButton : Node2D
{
	public bool Disable { get; set; }
	public int SecondCounter { get; private set; } = 0;
	public int MaxTime { get; private set; }
	public bool IsPaused { get; private set; }
	private Node2D _parent;
	private Timer _abilityTimer;
	private int _abilityIndex;
	private TextureRect _abilityReadyPic;
	private TextureRect _abilityNotReadyPic;
	private TouchScreenButton _abilityButton;
	private Label _timeLeftLabel;
	private CharacterManager _characterManager;
	private AbilityBase abilityBase;
	private BoostDexterity boostDexterity = new BoostDexterity();
	private BoostIntelligence boostIntelligence = new BoostIntelligence();
	private BoostStrength boostStrength = new BoostStrength();
	private SpeedUp dash = new SpeedUp();
	private Shield shield = new Shield();
	private FireBlast fireBlast = new FireBlast();
	private ArrowRain arrowRain = new ArrowRain();
	private DeadlyStrike deadlyStrike = new DeadlyStrike();
	private string textureActive = "res://AppIcon/AbilityIcons/PNG/Activated/BoostDexterityIcon.png";
	private string textureNotActive = "res://AppIcon/AbilityIcons/PNG/Activated/BoostDexterityIcon.png";

	public override void _Ready()
	{
		_timeLeftLabel = GetNode<Label>("TimeLeft");
		_abilityReadyPic = GetNode<TextureRect>("%AbilityReadyPic");
		_abilityNotReadyPic = GetNode<TextureRect>("%AbilityNotReadyPic");
		_parent = GetParent<Node2D>();
		_abilityButton = GetNodeOrNull<TouchScreenButton>("%TouchAbilityButton");
		Debug.Print(_abilityButton.GetInstanceId().ToString());
		//GetNode<TouchScreenButton>("TouchAbilityButton");
		_characterManager = GetNode<CharacterManager>("/root/CharacterManager");
		var selectedId = _characterManager.LoadLastSelectedCharacterID();

		if (_parent is DefaultPlayer player)
		{
			_abilityIndex = selectedId * 10 + _characterManager.LoadAbilityChosenByID(selectedId.ToString());

			//I am sorry for this code
			MaxTime = _abilityIndex switch
			{
				11 => boostDexterity.getCooldown(),
				12 => arrowRain.getCooldown(),
				21 => dash.getCooldown(),
				22 => deadlyStrike.getCooldown(),
				31 => boostStrength.getCooldown(),
				32 => shield.getCooldown(),
				41 => boostIntelligence.getCooldown(),
				42 => fireBlast.getCooldown(),
				_ => 0
			};
			textureActive = _abilityIndex switch
			{
				11 => "res://AppIcon/AbilityIcons/PNG/Activated/BoostDexterityIcon.png",
				12 => "res://AppIcon/AbilityIcons/PNG/Activated/ArrowRainIcon.png",
				21 => "res://AppIcon/AbilityIcons/PNG/Activated/DashIcon.png",
				22 => "res://AppIcon/AbilityIcons/PNG/Activated/DeadlyStrikeIcon.png",
				31 => "res://AppIcon/AbilityIcons/PNG/Activated/BoostStrengthIcon.png",
				32 => "res://AppIcon/AbilityIcons/PNG/Activated/ShieldIcon.png",
				41 => "res://AppIcon/AbilityIcons/PNG/Activated/BoostIntelligenceIcon.png",
				42 => "res://AppIcon/AbilityIcons/PNG/Activated/FireBlastIcon.png",
				_ => ""
			};
			textureNotActive = _abilityIndex switch
			{
				11 => "res://AppIcon/AbilityIcons/PNG/Deactivated/BoostDexterityIcon.png",
				12 => "res://AppIcon/AbilityIcons/PNG/Deactivated/ArrowRainIcon.png",
				21 => "res://AppIcon/AbilityIcons/PNG/Deactivated/DashIcon.png",
				22 => "res://AppIcon/AbilityIcons/PNG/Deactivated/DeadlyStrikeIcon.png",
				31 => "res://AppIcon/AbilityIcons/PNG/Deactivated/BoostStrengthIcon.png",
				32 => "res://AppIcon/AbilityIcons/PNG/Deactivated/ShieldIcon.png",
				41 => "res://AppIcon/AbilityIcons/PNG/Deactivated/BoostIntelligenceIcon.png",
				42 => "res://AppIcon/AbilityIcons/PNG/Deactivated/FireBlastIcon.png",
				_ => ""
			};
		}
		Debug.Print(textureActive);
		Debug.Print(textureNotActive);

		_timeLeftLabel.Text = MaxTime.ToString();
		_abilityTimer = GetNode<Timer>("AbilityTimer");
		_abilityReadyPic.Texture.TakeOverPath(textureActive);
		_abilityNotReadyPic.Texture.TakeOverPath(textureNotActive);
		_abilityTimer.Timeout += OnTimerTimeout;
		SecondCounter = MaxTime;
		Disable = true;
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
			_abilityReadyPic.Show();
			_abilityNotReadyPic.Hide();
		}
		_timeLeftLabel.Text = (MaxTime - SecondCounter).ToString();
		IsPaused = _abilityTimer.Paused;
	}

	public void _on_touch_ability_button_pressed()
	{
		if (!Disable)
		{
			//call to the chars ability here
			if (_parent is DefaultPlayer player)
			{
				Disable = true;
				player.UseAbility();
				//Debug.Print(MaxTime.ToString());
			}
			_abilityReadyPic.Hide();
			_abilityNotReadyPic.Show();
			_abilityTimer.Start();
			_timeLeftLabel.Show();
		}
	}
}
