using Godot;

/// <summary>
/// Represents an enemy projectile that moves in a given direction,
/// detects collisions, and applies damage to the player.
/// </summary>
public partial class EnemyProjectile : Area2D
{
    [Export] public float Speed = 25f;
    [Export] public float Damage = 1f;

    private Vector2 direction;

    /// <summary>
    /// Initializes the projectile with a direction to the player.
    /// </summary>
    /// <param name="dir">The direction in which the projectile should move.</param>
    public void Initialize(Vector2 dir)
    {
        direction = dir.Normalized();
        GD.Print($"Projectile initialized with direction: {direction}");
    }

    public override void _Ready()
    {
        Connect("body_entered", new Callable(this, nameof(_on_EnemyProjectile_body_entered)));
    }

    public override void _PhysicsProcess(double delta)
    {
        Position += direction * Speed * (float)delta;

        if (!GetViewportRect().HasPoint(GlobalPosition))
        {
            QueueFree();
        }
    }

    private void _on_EnemyProjectile_body_entered(Node body)
    {
        GD.Print($"Collision detected with: {body.Name}");

        if (body is DefaultPlayer player)
        {
            GD.Print("Player detected! Applying damage...");
            var health = player.GetNodeOrNull<Health>("Health");

            if (health != null)
            {
                health.Damage(Damage);
                GD.Print($"EnemyProjectile dealt {Damage} damage to the player!");
            }
            else
            {
                GD.Print("Health node not found on player!");
            }

            QueueFree();
        }
        else
        {
            GD.Print("Collision occurred, but not with the player.");
        }
    }
}