using Godot;
using System;

public partial class JoystickControl : Control
{
	public Vector2 PosVector { get; set; } = Vector2.Zero;

	[Export]
	public float Deadzone { get; set; } = 15.0f;
}
