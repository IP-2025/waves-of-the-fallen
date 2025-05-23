using Godot;
using System;

public partial class StoneAttack : Area2D
{
    
	[Export] public float Speed = 300.0f;
	[Export] public float Lifetime = 1.5f;

	private float Damage;
	private Vector2 direction;
	private float lifetimeTimer = 0.0f;
    private AnimatedSprite2D animatedSprite2D;
    private bool attack=false;

    public void Initialize(Vector2 dir, float damage)
    {
        direction = dir.Normalized();
        Damage = damage;

        Rotation = direction.Angle();
        animatedSprite2D.FlipV = direction.X < 0;
        
    }

	public override void _Ready()
	{
        animatedSprite2D=GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		Connect("body_entered", new Callable(this, nameof(_on_EnemyProjectile_body_entered)));
	}

	public override void _PhysicsProcess(double delta)
	{
        if (!attack) {
            Position += direction * Speed * (float)delta;
            lifetimeTimer += (float)delta;
            if (animatedSprite2D.Animation != "travel")
            animatedSprite2D.Play("travel");
        }
        
        if (lifetimeTimer >= Lifetime)
            {
                attack = true;
                playAttackAnimnation();
            }
        
	}

    private void playAttackAnimnation()
    {
        Speed = 0;
        animatedSprite2D.Play("attack");
     }
    private void OnAnimationFinished()
    {
        attack = false;
       QueueFree();
    }

	/// <summary>
    /// Handles collision with other objects.
    /// Applies damage to the player if hit and removes the projectile.
    /// </summary>
    private void _on_EnemyProjectile_body_entered(Node body)
    {
        if (body is DefaultPlayer player)
        {
            var health = player.GetNodeOrNull<Health>("Health");
            if (health != null)
            {
                health.Damage(Damage);
            }
            QueueFree();
        }
        else if (body is StaticBody2D || body is TileMap)
        {
            QueueFree();
        }
    }
}
