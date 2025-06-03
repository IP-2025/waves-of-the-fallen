using Godot;
using System;
public partial class DoubleBladeL : MeleeWeapon
{
	private AnimationPlayer DoubleBladeLAnimationPlayer;
	private Sprite2D SwordTrailTest;
	
	private const string _resBase = "res://Weapons/Melee/DoubleBlades/";

	public override int DefaultPiercing { get; set; } = 0;
	public override float DefaultSpeed { get; set; } = 0f;
	public override string ResourcePath => _resBase + "Resources/";
	public override string IconPath => ResourcePath + "DoubleBlades.png";
	public override float DefaultRange { get; set; } = 150f;
	public override int DefaultDamage { get; set; } = 150;
	
	public override float ShootDelay{ get; set; } = 1f;

	public override void _Ready()
	{
		DoubleBladeLAnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayerL");
		SwordTrailTest = GetNode<Sprite2D>("SwordTrailTest");
		SwordTrailTest.Visible = false;
	}

	public void StartAttack()
	{ 
		ShootMeleeVisual(() =>
	{
		DoubleBladeLAnimationPlayer.Play("BladeLAttack");
	});
	}
	
}
