using Godot;
using System;
using System.Linq;

public abstract partial class MeleeWeapon : Area2D
{
	protected AnimatedSprite2D animatedSprite;
	[Export] public float WeaponRange = 50f;
	[Export] public int MeleeDamage = 50;
	Node target;
	//protected PackedScene projectileScene = null;
	public override void _Ready()
	{
	}
		
	public override void _PhysicsProcess(double delta)
	{
		target = FindNearestEnemy();
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
	protected void MeleeAttack()
	{
		var healthNode = target.GetNodeOrNull<Health>("Health");
		
		if (healthNode != null)
		{
			healthNode.Damage(MeleeDamage);
			GD.Print("Error");
		}
	}
	protected void ShootMeleeVisual()
	{
		if (TryGetPosition(target, out var position))
		{
			var tween = CreateTween();
			//move forward
			tween.TweenProperty(this, "global_position", position, 0.1)
				.SetTrans(Tween.TransitionType.Sine)
				.SetEase(Tween.EaseType.Out);

			//Call method for attack
			tween.TweenCallback(Callable.From(() => {
				GD.Print("Execute Attack");
				MeleeAttack(); 
			}));
			//go back
			tween.TweenProperty(this, "position", Vector2.Zero, 0.1)
				.SetDelay(0.1)
				.SetTrans(Tween.TransitionType.Sine)
				.SetEase(Tween.EaseType.In);
		}
	}
}
