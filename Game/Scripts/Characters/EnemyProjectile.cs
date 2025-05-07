using Godot;

/// <summary>
/// Represents an enemy projectile that moves in a given direction,
/// detects collisions, and applies damage to the player.
/// 
/// Configuration:
/// - Speed: Movement speed of the projectile.
/// - Damage: Damage dealt to the player upon collision.
/// 
/// Behavior:
/// - Moves in the initialized direction.
/// - Detects collisions with the player and applies damage.
/// - Removes itself after hitting the player.
/// </summary>
public partial class EnemyProjectile : Area2D
{
    [Export] public float Speed = 1000f;
    private float Damage;

    private Vector2 direction;

    public void Initialize(Vector2 dir, float damage)
    {
        direction = dir.Normalized();
        Damage = damage; // Damage from specific enemy type
        GD.Print($"Projectile initialized with direction: {direction} and damage: {Damage}");
    }

    public override void _Ready()
    {
        Connect("body_entered", new Callable(this, nameof(_on_EnemyProjectile_body_entered)));
    }

    public override void _PhysicsProcess(double delta)
    {
        Position += direction * Speed * (float)delta;
    }

    /// <summary>
    /// Handles collision with other objects.
    /// Applies damage to the player if hit and removes the projectile.
    /// </summary>
    /// <param name="body">The object the projectile collided with.</param>
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