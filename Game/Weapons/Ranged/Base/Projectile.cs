using Godot;
using System;

public abstract partial class Projectile : Area2D
{
	protected float Speed { get; set; } = 1000;
	protected int Piercing { get; set; } = 1;
	protected int Damage { get; set; } = 100;

	public int dex, str, @int;

	public override void _PhysicsProcess(double delta)
	{
		var direction = Vector2.Right.Rotated(Rotation);

		Position += direction * Speed * (float)delta;

	}

	public virtual void OnBodyEntered(Node2D body)
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
