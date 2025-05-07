using Godot;
using System;

public abstract partial class EnemyBase : CharacterBody2D
{
	// Can be adapted in inspector for each deriving enemy
	[Export] public float speed = 200f;
	[Export] public float damage = 10f;
	[Export] public float attacksPerSecond = 1.5f;
	[Export] private NodePath animationPath;
	
	public DefaultPlayer player { get; set; }
	protected float attackCooldown;
	protected float timeUntilAttack;
	protected bool withinAttackRange = false;
	protected AnimatedSprite2D animation;
	protected enum EnemyAnimationState { IdleOrWalk, Attack, Hit, Die }
	protected EnemyAnimationState currentState = EnemyAnimationState.IdleOrWalk;

	public abstract void Attack(); 

	public override void _Ready()
	{
		attackCooldown = 1f / attacksPerSecond;
		timeUntilAttack = attackCooldown;
		
		if (animationPath != null)
		{
			animation = GetNode<AnimatedSprite2D>(animationPath);
			animation.AnimationFinished += OnAnimationFinished;
		}
	}

	public override void _Process(double delta)
	{
		if (currentState == EnemyAnimationState.Die)
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
		
		HandleMovement(direction);
		MoveAndSlide();
		// For choosing right animation:
		UpdateAnimationState();
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
	
	/// Animation stuff
	
	protected void PlayIfNotPlaying(string animName)
	{
		if (animation.Animation != animName)
		{
			animation.Play(animName);
		}	
	}
	
	protected void PlayWalkOrIdle()
	{
		if (Velocity.Length() > 0.1f)
		{
			PlayIfNotPlaying("walk");
		}
		else
		{
			PlayIfNotPlaying("idle");
		}
	}
	
	public virtual void OnHit()
	{
		if (currentState == EnemyAnimationState.Die)
		{
			return;
		}
			
		currentState = EnemyAnimationState.Hit;
		PlayIfNotPlaying("hit");
	}

	public virtual void OnDeath()
	{
		if (currentState == EnemyAnimationState.Die)
		{
			return;
		}
			
		currentState = EnemyAnimationState.Die;
		Velocity = Vector2.Zero; 
		PlayIfNotPlaying("death");
	}
	
	protected virtual void OnAnimationFinished()
	{
		if (animation.Animation == "hit")
		{
			currentState = EnemyAnimationState.IdleOrWalk;
		}
		else if (animation.Animation == "death")
		{
			QueueFree(); 
		}
	}
	
	protected void HandleAnimations()
	{
		switch (currentState)
		{
			case EnemyAnimationState.Die:
			case EnemyAnimationState.Hit:
				break;
			case EnemyAnimationState.Attack:
				PlayIfNotPlaying("attack");
				break;
			case EnemyAnimationState.IdleOrWalk:
				PlayWalkOrIdle();
				break;
		}
	}
	
	protected void UpdateAnimationState()
	{
		if (currentState == EnemyAnimationState.Hit || currentState == EnemyAnimationState.Die)
		{
			return;
		}
		
		if (withinAttackRange)
		{
			currentState = EnemyAnimationState.Attack;
		}
		else
		{
			currentState = EnemyAnimationState.IdleOrWalk;
		}
		HandleAnimations();
	}
}
