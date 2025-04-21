using Godot;
using System;

public partial class CrossbowArrow : Area2D
{
	
	private float _travelDistance = 0;
	public override void _PhysicsProcess(double delta)
	{
		const int speed = 1200;
		const float range = 1000;
		var direction = Vector2.Right.Rotated(Rotation);

		Position += direction * speed * (float)delta;

		_travelDistance += speed * (float)delta;

		if (_travelDistance >= range)
		{
			QueueFree();
		}


	}
	
	public void OnBodyEntered(Node2D body) 
	{
		QueueFree();
		
		var healthNode = body.GetNodeOrNull<Health>("Health");

		if (healthNode != null)
		{
			//Set in Coment because, it accesed the health class for the player.
			//Temporarly no enemie kill possible.
			//healthNode.Damage(100);
		}
	}

	
	
}
