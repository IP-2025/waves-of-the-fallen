using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public const float Speed = 8.0f;
	public const float JumpVelocity = 5.5f;
	private Camera3D camera;

	public override void _Ready()
	{
		GD.Print($"Player Node: Name={Name}, localID={Multiplayer.GetUniqueId()}, authority={GetMultiplayerAuthority()}");
		
		if (!IsMultiplayerAuthority())
		{
			return;
		}
		camera = GetNode<Camera3D>("Camera3D");
		camera.Current = true;
	}


	public override void _EnterTree()
	{
		SetMultiplayerAuthority(int.Parse(Name));
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsMultiplayerAuthority())
		{
			return;
		}

		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta * 1.5f;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "up", "down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
