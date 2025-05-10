using Godot;
using System;

public abstract partial class MeleeWeapon : WeaponBase
{
	[Export] public float AttackCooldown = 0.5f;
	[Export] public float MeleeRange = 30f;
	private float cooldown = 0f;
	// Called when the node enters the scene tree for the first time.
	protected override void OnTargetInSight(Node target, Vector2 enemyPosition)
	{
		if(cooldown <= 0 && IsInRange(enemyPosition))
		{
			MeleeAttack(target);
			cooldown = AttackCooldown;
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		cooldown -= (float) delta;
		base._PhysicsProcess(delta);
	}
	protected bool IsInRange(Vector2 targetPos)
	{
		return GlobalPosition.DistanceTo(targetPos) <= MeleeRange;
	}
	protected abstract void MeleeAttack(Node target);
}
