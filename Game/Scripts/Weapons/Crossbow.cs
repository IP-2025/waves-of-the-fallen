using Godot;
using System;
using System.Linq; // Needed for LINQ

public partial class Crossbow : RangedWeapon
{
    
    private PackedScene _arrowScene = GD.Load<PackedScene>("res://Scenes/Weapons/crossbow_arrow.tscn");
    
    public override void _Ready()
    {
        _animatedSprite = GetNode<AnimatedSprite2D>("./WeaponPivot/CrossbowSprite");
		
    }

    protected override void Shoot()
    {
        Area2D arrowInstance = _arrowScene.Instantiate() as Area2D;
        Marker2D shootingPoint = GetNode<Marker2D>("%CrossbowShootingPoint");
        arrowInstance.GlobalPosition = shootingPoint.GlobalPosition;
        arrowInstance.GlobalRotation = shootingPoint.GlobalRotation;
        
        GetTree().CurrentScene.AddChild(arrowInstance);

    }
    
    public async void OnTimerTimeoutCrossbow()
    {
        _animatedSprite.Play("shoot");
        await ToSignal(GetTree().CreateTimer(0.27), "timeout");
        
        Shoot();
    }
    
}