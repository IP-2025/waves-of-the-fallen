using Godot;
using System;
using System.Diagnostics;

public partial class BasicEnemy : EnemyBase
{
	private AnimatedSprite2D animation;
	
	public override void _Ready()
	{
		base._Ready();
		animation = GetNode<AnimatedSprite2D>("GoblinAnimation");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		FindNearestPlayer();
		if (player != null)
		{
			LookAt(player.GlobalPosition);
			Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
			Velocity = direction * speed;
			
			if (Velocity.Length() > 0.1f)
			{
				PlayIfNotPlaying("walk");
			}
			else
			{
				PlayIfNotPlaying("idle");
			}
		}
		else
		{
			Velocity = Vector2.Zero;
		}

		MoveAndSlide();
	}

	public override void Attack() 
	{
		player.GetNode<Health>("Health").Damage(damage);
		Debug.Print("BasicEnemy attacks (melee)!");
	}
	
	private void PlayIfNotPlaying(string animName)
	{
		if (animation.Animation != animName)
		{
			animation.Play(animName);
		}	
	}
}
