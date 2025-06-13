using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class DeadlyStrike : AbilityBase
{
	private float Damage { get; set; } = 200;
	private float Range { get; set; } = 200;
	bool killed = true;
	private int countAttacks = 5;
	private int cooldown = 3;

	public override void _Ready()
	{
		DoDeadlyStrikeAsync();
	}

	public async Task DoDeadlyStrikeAsync()
	{
		while (countAttacks > 0 && killed == true)
		{
			var target = FindNearestEnemy();
			if (target != null && TryGetPosition(target, out var position))
			{
				LookAt(position);
			}
			await ToSignal(GetTree().CreateTimer(0.15), "timeout");
			MeleeAttack(target);
			countAttacks--;
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
			if (enemyNode.GetNode<Health>("Health").health <= 0)
				continue;

			float dist = GlobalPosition.DistanceTo(enemyNode.GlobalPosition);

			if (dist < closestDist && dist <= Range)
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
		var healthNode = actualTarget.GetNodeOrNull<Health>("Health");
		if (healthNode != null)
		{
			Debug.Print((healthNode.MaxHealth - Damage).ToString());
			if (healthNode.MaxHealth - Damage <= 0)
			{
				killed = true;
			}
			else
			{
				killed = false;
			}
			healthNode.Damage(Damage);
			SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("deadlyStrike"), GlobalPosition, -5);
		}
	}

	public override int getCooldown()
	{
		return cooldown;
	}
}
