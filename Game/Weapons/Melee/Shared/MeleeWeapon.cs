using Godot;
using System;
using System.Linq;

public abstract partial class MeleeWeapon : Area2D
{
	protected AnimatedSprite2D animatedSprite;
	[Export] public float WeaponRange = 50f;
	[Export] public int MeleeDamage = 50;
	[Export] public float Speed = 0.2f;
	bool onCooldown = false;
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
	protected void MeleeAttack(Node actualTarget)
	{
			if(actualTarget == null){
				return;
			}
			Godot.GD.PushError(actualTarget);
			var healthNode = actualTarget.GetNodeOrNull<Health>("Health");
			if (healthNode != null)
			{
			healthNode.Damage(MeleeDamage);
			}
	}
	protected void ShootMeleeVisual(Action onAttackComplete = null)
	{
		
		if(!onCooldown){
			onCooldown = true;
			Node actualTarget = FindNearestEnemy();
			if (actualTarget == null){
			onCooldown = false;
			return;
			}
				if(TryGetPosition(actualTarget, out var position))
				{
				var tween = CreateTween();
				//move forward
				tween.TweenProperty(this, "global_position", position, Speed)
					.SetTrans(Tween.TransitionType.Sine)
					.SetEase(Tween.EaseType.Out);

				//Call method for attack and Animation
				tween.TweenCallback(Callable.From(() => {
					if (!Godot.GodotObject.IsInstanceValid(actualTarget))
					return; //Prevents rare asyncronous problem 
					onAttackComplete?.Invoke();
					MeleeAttack(actualTarget);
					onCooldown = false;
					//};
				}));
				//go back
				tween.TweenProperty(this, "position", Vector2.Zero, Speed)
					.SetDelay(0.1)
					.SetTrans(Tween.TransitionType.Sine)
					.SetEase(Tween.EaseType.In);
			}
		}
	}
}
