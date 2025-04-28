using Godot;
using System;
using System.Diagnostics;

public partial class Main : Node2D
{
	public bool enableDebug = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void DebugIt(string message)
	{
		if (enableDebug)
		{
			Debug.Print(message);
		}
	}
}
