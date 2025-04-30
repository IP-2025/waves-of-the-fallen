using Godot;
using System;
using System.Linq; // Needed for LINQ

public partial class Crossbow : Area2D
{
	
	private AnimatedSprite2D _animatedSpride;
	private PackedScene _arrowScene = GD.Load<PackedScene>("res://Scenes/Weapons/crossbow_arrow.tscn");
	
	public override void _Ready()
	{
		_animatedSpride = GetNode<AnimatedSprite2D>("./WeaponPivot/CrossbowSprite");
		
	}
	public override void _PhysicsProcess(double delta)
	{
		var bodies = GetOverlappingBodies();

		var nearest = bodies
			.Select(body => new
			{
				Body = body,
				Position = TryGetPosition(body, out var pos) ? pos : (Vector2?)null
			})
			.Where(x => x.Position != null)
			.OrderBy(x => GlobalPosition.DistanceTo(x.Position.Value))
			.FirstOrDefault();

		if (nearest != null)
		{
			LookAt(nearest.Position.Value);
		}
	}
	
	private bool TryGetPosition(object body, out Vector2 position)
	{
		position = Vector2.Zero;
		if (body is GodotObject obj)
		{
			Variant globalPosVariant = obj.Get("global_position");
			if (globalPosVariant.VariantType == Variant.Type.Vector2)
			{
				position = (Vector2)globalPosVariant;
				return true;
			}

			Variant posVariant = obj.Get("position");
			if (posVariant.VariantType == Variant.Type.Vector2)
			{
				position = (Vector2)posVariant;
				return true;
			}
		}
		return false;
	}

	private void Shoot()
	{
		Area2D arrowInstance = _arrowScene.Instantiate() as Area2D;
		Marker2D shootingPoint = GetNode<Marker2D>("%CrossbowShootingPoint");
		arrowInstance.GlobalPosition = shootingPoint.GlobalPosition;
		arrowInstance.GlobalRotation = shootingPoint.GlobalRotation;
		
		GetTree().CurrentScene.AddChild(arrowInstance);

	}
	
	public async void OnTimerTimeout()
	{
		_animatedSpride.Play("shoot");
		await ToSignal(GetTree().CreateTimer(0.27), "timeout");
		
		Shoot();
	}

}
