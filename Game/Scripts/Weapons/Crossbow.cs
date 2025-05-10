using Godot;
using System;
using System.Linq; // Needed for LINQ

public partial class Crossbow : RangedWeapon
{
	
	private PackedScene arrowScene = GD.Load<PackedScene>("res://Scenes/Weapons/crossbow_arrow.tscn");
	
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/CrossbowSprite");
		projectileScene = arrowScene;
		WeaponRange = 700f;
	}
	
	public async void OnTimerTimeoutCrossbow()
	{
		animatedSprite.Play("shoot");
		await ToSignal(GetTree().CreateTimer(0.2), "timeout");
		
		Shoot();
	}
	
}
