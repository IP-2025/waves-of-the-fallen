using Godot;
using System;
using System.Diagnostics;

public abstract partial class EnemyBase : CharacterBody2D
{
	public bool enableDebug = false;
	// Can be adapted in inspector for each deriving enemy
	[Export] public float speed = 200f;
	[Export] public float damage = 10f;
	[Export] public float attacksPerSecond = 1.5f;
	[Export] private NodePath animationPath;
	
	public DefaultPlayer player { get; set; }
	protected float attackCooldown;
	protected float timeUntilAttack;
	protected bool withinAttackRange = false;
	
	private AnimationHandler animationHandler;
	private AnimatedSprite2D animation;

	public abstract void Attack(); 

	public override void _Ready()
	{
		attackCooldown = 1f / attacksPerSecond;
		timeUntilAttack = attackCooldown;
		
		// Notice: AnimationPath HAS to be set for every enemy in its inspektor
		// Animations needed: walk, idle, death, attack, hit
		if (animationPath != null)
		{
			animation = GetNode<AnimatedSprite2D>(animationPath);
			animationHandler = new AnimationHandler(animation);
			animationHandler.OnDeathAnimationFinished += () => QueueFree();
		}
		else
		{
			GD.PushError($"{Name} has no animationPath set!");
		}
	}

	public override void _Process(double delta)
	{
		if (animationHandler.IsDying)
		{
			Velocity = Vector2.Zero;
			MoveAndSlide();
			return;
		}
		
		FindNearestPlayer();
		
		if (player == null)
		{
			Velocity = Vector2.Zero;
			MoveAndSlide();
			return;
		}
		
		Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
		if (player != null)
		{
			if (animation != null)
			{
				animation.FlipH = direction.X < 0;
			}
		}
		
		if (withinAttackRange && timeUntilAttack <= 0f)
		{
			Attack();
			timeUntilAttack = attackCooldown;
		}
		else
		{
			timeUntilAttack -= (float)delta;
		}
		
		HandleMovement(direction.Normalized());
		MoveAndSlide();
		// Update Animations
		animationHandler.UpdateAnimationState(withinAttackRange, Velocity);
	}
	
	protected virtual void HandleMovement(Vector2 direction)
	{
		// normal behavior: Meele-enemy, other classes can adapt
		Velocity = direction.Normalized() * speed;
	}

	public virtual void OnAttackRangeBodyEnter(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			withinAttackRange = true;
			DebugIt("Player entered range.");
		}
	}

	public virtual void OnAttackRangeBodyExit(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			withinAttackRange = false;
			timeUntilAttack = attackCooldown;
			DebugIt("Player left range.");
		}
	}
	
	public void OnHit()
	{
		animationHandler.SetHit();
	}

	public void OnDeath()
	{
		Velocity = Vector2.Zero;
		animationHandler.SetDeath();
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

	    private void DebugIt(string message)
    {
        if (enableDebug) Debug.Print("EnemyBase: " + message);
    }
}
