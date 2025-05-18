using Godot;
using System;

public partial class AnimationHandler 
{
	public enum EnemyAnimationState { IdleOrWalk, Attack, Hit, Die }
	public EnemyAnimationState CurrentState { get; private set; } = EnemyAnimationState.IdleOrWalk;
	public bool IsDying => CurrentState == EnemyAnimationState.Die;
	public bool IsHit => CurrentState == EnemyAnimationState.Hit;
	public event Action OnDeathAnimationFinished;
	
	private AnimatedSprite2D animation;
	
	public AnimationHandler(AnimatedSprite2D animation)
	{
		this.animation = animation;
		this.animation.AnimationFinished += OnAnimationFinished;
	}
	
	public void PlayIfNotPlaying(string animName)
	{
		if (animation.Animation != animName)
		{
			animation.Play(animName);
		}	
	}
	
	public void PlayWalkOrIdle(Vector2 velocity)
	{
		if (velocity.Length() > 0.1f)
		{
			PlayIfNotPlaying("walk");
		}
		else
		{
			PlayIfNotPlaying("idle");
		}
	}
	
	public void SetHit()
	{
		if (CurrentState == EnemyAnimationState.Die)
		{
			return;
		}
			
		CurrentState = EnemyAnimationState.Hit;
		PlayIfNotPlaying("hit");
	}

	public void SetDeath()
	{
		if (CurrentState == EnemyAnimationState.Die)
		{
			return;
		}
			
		CurrentState = EnemyAnimationState.Die;
		PlayIfNotPlaying("death");
	}
	
	public void UpdateAnimationState(bool withinAttackRange, Vector2 velocity)
	{
		if (CurrentState == EnemyAnimationState.Hit || CurrentState == EnemyAnimationState.Die)
		{
			return;
		}

		CurrentState = withinAttackRange
			? EnemyAnimationState.Attack
			: EnemyAnimationState.IdleOrWalk;

		HandleAnimations(velocity);
	}
	
	private void OnAnimationFinished()
	{
		if (animation.Animation == "hit")
		{
			CurrentState = EnemyAnimationState.IdleOrWalk;
		}
		else if (animation.Animation == "death")
		{
			OnDeathAnimationFinished?.Invoke(); 
		}
	}
	
	private void HandleAnimations(Vector2 velocity)
	{
		switch (CurrentState)
		{
			case EnemyAnimationState.Attack:
				PlayIfNotPlaying("attack");
				break;
			case EnemyAnimationState.IdleOrWalk:
				PlayWalkOrIdle(velocity);
				break;
		}
	}
}
