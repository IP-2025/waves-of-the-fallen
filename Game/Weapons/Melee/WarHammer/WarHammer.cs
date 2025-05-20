using Godot;
using System;

public partial class WarHammer : RangedWeapon
{
	private PackedScene throwHammerScene = GD.Load<PackedScene>("res://Weapons/Melee/WarHammer/WarHammer.tscn");
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/WarHammer");
		projectileScene = throwHammerScene;
		WeaponRange = 300f;
	}
	/*Task: Erstelle etwas Ähnliches wie Thors Hammer. 
	Er soll auf die Gegner fliegen könnnen und dort AOE Damage in kleinen Bereich
	machen. Balancing wird später noch relevant - sehr große Ähnlichkeit mit dem
	Feuerstab. Hohe Kraft AOE, Hohe Reichweite, Hoher Cooldwon. Hammer?
	*/
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	private async void OnTimerTimeoutAttackHammer()
	{ 
		var target = FindNearestEnemy();
		if (target == null)
			return;

		animatedSprite.Play("shoot");
		await ToSignal(GetTree().CreateTimer(1), "timeout");
		Shoot();
	}
}
