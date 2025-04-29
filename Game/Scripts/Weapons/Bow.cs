using Godot;
using System;

public partial class Bow : RangedWeapon
{
	private PackedScene _arrowScene = GD.Load<PackedScene>("res://Scenes/Weapons/bow_arrow.tscn");
    
	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/BowSprite");
		
	}
	protected override void Shoot()
	{
		Area2D arrowInstance = _arrowScene.Instantiate() as Area2D;
		Marker2D shootingPoint = GetNode<Marker2D>("WeaponPivot/BowSprite/BowShootingPoint");
		arrowInstance.GlobalPosition = shootingPoint.GlobalPosition;
		arrowInstance.GlobalRotation = shootingPoint.GlobalRotation;
        
		GetTree().CurrentScene.AddChild(arrowInstance);

	}
	
	public async void OnTimerTimeoutBow()
	{
		var target = FindNearestEnemy();
		if (target == null)
			return;

		_animatedSprite.Play("shoot");
		await ToSignal(GetTree().CreateTimer(0.13), "timeout");

		Shoot();
	}
	
}
