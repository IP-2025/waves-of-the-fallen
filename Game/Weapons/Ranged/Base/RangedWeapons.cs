using Godot;
using System;
using System.Linq;

public abstract partial class RangedWeapon : Area2D
{
	protected AnimatedSprite2D animatedSprite;
	protected float WeaponRange = 800f;
	protected PackedScene projectileScene = null;

	public override void _PhysicsProcess(double delta)
	{
		var target = FindNearestEnemy();
		if (target != null && TryGetPosition(target, out var position))
		{
			LookAt(position);
		}
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

			if (dist < closestDist && dist <= WeaponRange)
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
		Area2D projectileInstance = projectileScene.Instantiate() as Area2D;
		Marker2D shootingPoint = GetNode<Marker2D>("ShootingPoint");
		projectileInstance.GlobalPosition = shootingPoint.GlobalPosition;
		projectileInstance.GlobalRotation = shootingPoint.GlobalRotation;
		
		GetTree().CurrentScene.AddChild(projectileInstance);

	}

	
}
