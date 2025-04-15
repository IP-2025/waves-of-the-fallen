using Godot;
using System;

// TODO: Implement base class for enemies
public partial class RangedEnemy : CharacterBody2D
{
	[Export] public float speed = 200f;
	[Export] public float stop_distance = 350f;

	private DefaultPlayer player;

	public override void _Ready()
	{
		player = GetNode<DefaultPlayer>("/root/Node2D/DefaultPlayer");
		AddToGroup("ranged_enemies");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (player == null)
		{
			Velocity = Vector2.Zero;
			MoveAndSlide();
			return;
		}

		float dist = GlobalPosition.DistanceTo(player.GlobalPosition);
		LookAt(player.GlobalPosition);

		if (dist > stop_distance)
		{
			Vector2 toPlayer = (player.GlobalPosition - GlobalPosition).Normalized();
			Velocity = toPlayer * speed;
		}
		else
		{
			Velocity = Vector2.Zero;
		}

		MoveAndSlide();
	}

	public void Attack()
	{
		GD.Print("Ranged attack!");
	}
}
