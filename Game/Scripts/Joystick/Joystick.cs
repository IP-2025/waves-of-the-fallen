using Godot;
using System;

public partial class Joystick : Node2D
{
	[Export]
	public Vector2 PosVector { get; set; } = Vector2.Zero;

	[Export]
	public float Deadzone { get; set; } = 15.0f;
}