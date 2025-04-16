using Godot;
using System;

public abstract partial class EnemyBase : CharacterBody2D
{
	// Can be adapted in inspector for each deriving enemy
	[Export] public float speed = 200f;
	[Export] public float damage = 10f;
	[Export] public float attacksPerSecond = 1.5f;

	public DefaultPlayer player { get; set; }
	protected float attackCooldown;
	protected float timeUntilAttack;
	protected bool withinAttackRange = false;

	public override void _Ready()
	{
		attackCooldown = 1f / attacksPerSecond;
		timeUntilAttack = attackCooldown;
	}

	public override void _Process(double delta)
	{
		if (withinAttackRange && timeUntilAttack <= 0f)
		{
			Attack();
			timeUntilAttack = attackCooldown;
		}
		else
		{
			timeUntilAttack -= (float)delta;
		}
	}

	public abstract void Attack(); 

	public virtual void OnAttackRangeBodyEnter(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			withinAttackRange = true;
			GD.Print("Player entered range.");
		}
	}

	public virtual void OnAttackRangeBodyExit(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			withinAttackRange = false;
			timeUntilAttack = attackCooldown;
			GD.Print("Player left range.");
		}
	}
	
	protected void FindNearestPlayer()
	{
		float closestDist = float.MaxValue;
		DefaultPlayer closestPlayer = null;

		foreach (Node node in GetTree().GetNodesInGroup("player"))
		{
			if (node is not DefaultPlayer dp) continue;

			float dist = GlobalPosition.DistanceTo(dp.GlobalPosition);
			if (dist < closestDist)
			{
				closestDist = dist;
				closestPlayer = dp;
			}
		}

		player = closestPlayer;
	}

}
