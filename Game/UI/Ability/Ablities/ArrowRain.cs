using Godot;
using System;

public partial class ArrowRain : AbilityBase
{
	private const string _resBase = "res://Weapons/Ranged/MagicStaffs/Firestaff/";
	private const string _projectilePath = _resBase + "fireball.tscn";
	public float DefaultRange { get; set; } = 1000f;
	public int DefaultDamage { get; set; } = 500;
	public int DefaultPiercing { get; set; } = FireBall.DefaultPiercing;
	public float DefaultSpeed { get; set; } = 300;
	private int cooldown = 30;
	private static readonly PackedScene _fireballPacked = GD.Load<PackedScene>(_projectilePath);

	public override void _Ready()
	{
		
		var target = FindNearestEnemy();
		if (target != null && TryGetPosition(target, out var position))
			LookAt(position);
		Shoot();      
	}
	
	protected Node FindNearestEnemy()
	{
		float closestDist = float.MaxValue;
		Node closestEnemy = null;

		foreach (Node node in GetTree().GetNodesInGroup("enemies"))
		{
			if (node is not EnemyBase enemyNode)
				continue;

			float dist = GlobalPosition.DistanceTo(enemyNode.GlobalPosition);

			if (dist < closestDist && dist <= DefaultRange)
			{
				closestDist = dist;
				closestEnemy = enemyNode;
			}
		}
		return closestEnemy;
	}

	protected bool TryGetPosition(object body, out Vector2 position)
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

	protected void Shoot()
	{
		if (_fireballPacked == null) return;

		var projectileInstance = _fireballPacked.Instantiate<Area2D>();
		var shootingPoint = GetNode<Marker2D>("ShootingPoint");

		projectileInstance.GlobalPosition = shootingPoint.GlobalPosition;
		projectileInstance.GlobalRotation = shootingPoint.GlobalRotation;

		GetTree().CurrentScene.AddChild(projectileInstance);
	}

	public override int getCooldown()
	{
		return cooldown;
	}
}
