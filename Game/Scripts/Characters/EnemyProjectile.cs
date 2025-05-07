using Godot;

public partial class EnemyProjectile : Area2D
{
    [Export] public float Speed = 10f; // Geschwindigkeit des Projektils
    [Export] public float Damage = 1f; // Schaden, den das Projektil verursacht

    private Vector2 direction;

    /// <summary>
    /// Initialisiert das Projektil mit einer Richtung.
    /// </summary>
    /// <param name="dir">Die Richtung, in die das Projektil fliegen soll.</param>
    public void Initialize(Vector2 dir)
    {
        direction = dir.Normalized();
        GD.Print($"Projectile initialized with direction: {direction}");
    }

    public override void _PhysicsProcess(double delta)
    {
        // Bewege das Projektil
        Position += direction * Speed * (float)delta;

        // Entferne das Projektil, wenn es den Bildschirm verlässt
        if (!GetViewportRect().HasPoint(GlobalPosition))
        {
            QueueFree();
        }
    }

    private void _on_EnemyProjectile_body_entered(Node body)
    {
        GD.Print($"Collision detected with: {body.Name}");

        if (body is DefaultPlayer player) // Überprüfe, ob der Spieler getroffen wurde
        {
            GD.Print("Player detected! Applying damage...");
            
            // Versuche, den Health-Node zu finden
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

            // Zerstöre das Projektil nach dem Treffer
            QueueFree();
        }
        else
        {
            GD.Print("Collision occurred, but not with the player.");
        }
    }
}