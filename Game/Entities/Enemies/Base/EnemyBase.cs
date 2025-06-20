using Godot;
using System;
using System.Diagnostics;

public abstract partial class EnemyBase : CharacterBody2D
{
	public bool enableDebug = false;

	public float speed;
	public float damage;
	public float attacksPerSecond;
	[Export] private NodePath animationPath;
	public int scoreValue = 100;
	public DefaultPlayer player { get; set; }
	protected virtual float attackCooldown { get; set; }
	protected virtual float timeUntilAttack { get; set; }
	protected bool withinAttackRange = false;

	protected AnimationHandler animationHandler;
	private AnimatedSprite2D animation;

	public abstract void Attack();

	public override void _Ready()
	{
		attackCooldown = 1f / attacksPerSecond;
		timeUntilAttack = attackCooldown;

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

		if (animation != null)
		{
			animation.FlipH = direction.X < 0;
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

		animationHandler.UpdateAnimationState(withinAttackRange, Velocity);
	}

	protected virtual void HandleMovement(Vector2 direction)
	{
		float stopDistance = 0f; // Default stop distance, can be overridden by subclasses

		var stopDistProp = GetType().GetProperty("stopDistance");
		if (stopDistProp != null)
			stopDistance = (float)stopDistProp.GetValue(this);
		else
		{
			var stopDistField = GetType().GetField("stopDistance");
			if (stopDistField != null)
				stopDistance = (float)stopDistField.GetValue(this);
		}

		float dist = GlobalPosition.DistanceTo(player.GlobalPosition);

		if (dist > stopDistance)
		{
			Velocity = direction.Normalized() * speed;
		}
		else
		{
			Velocity = Vector2.Zero;

			// Pushback for enemy if player is too close
			if (dist < 40f)
			{
				Vector2 awayFromPlayer = (GlobalPosition - player.GlobalPosition).Normalized();
				Velocity = awayFromPlayer * speed * 0.5f;
			}
		}
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

		long myId = -1;
		if (player != null)
		{
			myId = player.OwnerPeerId;
			Game.Utilities.Backend.ScoreManager.OnEnemyKilled(myId, scoreValue);
		}

		var floatingScoreScene = GD.Load<PackedScene>("res://UI/FloatingScore/floating_score.tscn");
		var floatingScore = floatingScoreScene.Instantiate<FloatingScore>();

		int combo = Game.Utilities.Backend.ScoreManager.GetCombo(myId);

		if (combo > 1)
			floatingScore.Text = $"+{scoreValue} x{combo}";
		else
			floatingScore.Text = $"+{scoreValue}";

		floatingScore.SetPlayerColorById(myId);

		GetTree().Root.AddChild(floatingScore);
		floatingScore.GlobalPosition = GlobalPosition;

		GD.Print($"OnDeath: player={player}, scoreValue={scoreValue}");
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
				DebugIt($"Found player candidate: {dp.Name}, Distance: {dist}, Position: {dp.GlobalPosition}");
				closestDist = dist;
				closestPlayer = dp;
			}
		}

		player = closestPlayer;
	}

	private void DebugIt(string message)
	{
		if (enableDebug)
		{
			GD.Print("EnemyBase: " + message);
		}
	}
}
