using Godot;
using System;
using System.Linq;
//no use for now
public abstract partial class WeaponBase : Area2D
{
	protected AnimatedSprite2D animatedSprite;
	public override void _Ready()
	{
		//_animatedSprite = GetNodeOrNull<AnimatedSprite2D>("%Sprite");
	}
	public override void _PhysicsProcess(double delta)
	{
		var target = FindNearestEnemy();
		if (target != null && TryGetPosition(target, out var position))
		{
			LookAt(position);
			OnTargetInSight(target,position);
		}
	}
	protected abstract void OnTargetInSight(Node target, Vector2 targetPos);
	protected Node FindNearestEnemy()
	{
		float closestDist = float.MaxValue;
		Node closestEnemy = null;

		foreach (Node node in GetTree().GetNodesInGroup("Enemies"))
		{
			if (node is not EnemyBase enemyNode)
				continue;

			float dist = GlobalPosition.DistanceTo(enemyNode.GlobalPosition);
			if (dist < closestDist)
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
}
