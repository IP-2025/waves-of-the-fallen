using Godot;
using System;

public abstract partial class Projectile : Area2D
{
	protected int Damage = 100;
	protected int Piercing = 1;
	protected int Speed = 1000;
	
    public void OnBodyEntered(Node2D body) 
    {
        Piercing--;
        if (Piercing < 1)
        {
            QueueFree();
        }
    
        var healthNode = body.GetNodeOrNull<Health>("Health");
        if (healthNode != null)
        {
            healthNode.Damage(Damage);
        }
    }

	
}
