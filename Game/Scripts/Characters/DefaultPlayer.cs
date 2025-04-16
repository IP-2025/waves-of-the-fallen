using Godot;
using System;
using System.Diagnostics;
public partial class DefaultPlayer : CharacterBody2D
{
	[Export]
	public float Speed = 600.0f;
	public Node2D Joystick { get; set; }
	private Camera2D camera;
	private MultiplayerSynchronizer multiplayerSynchronizer;
	public bool enableDebug = false;
	public override void _Ready()
	{
		// set MultiplayerSynchronizer as authority of this id and then check authority against this id to see if player is authority
		// to later enable movement controlls and so on for the current player only
		multiplayerSynchronizer = GetNodeOrNull<MultiplayerSynchronizer>("MultiplayerSynchronizer");

		if (multiplayerSynchronizer != null)
		{
			DebugIt(("Trying to parse Name as int: ", Name).ToString());

			multiplayerSynchronizer.SetMultiplayerAuthority(int.Parse(Name));
			// only show joystic if this player is local player / has authority
			if (Multiplayer.GetUniqueId() == multiplayerSynchronizer.GetMultiplayerAuthority())
			{
				// find the joystic and load it
				var joystickScene = GD.Load<PackedScene>("res://Scenes/Joystick/joystick.tscn");
				Joystick = joystickScene.Instantiate<Node2D>();
				AddChild(Joystick); // // so that its only visible for the local player and others cant see ur joystic
			}
		}
		else
		{
			GD.PrintErr("Multiplayer Node was not found");
		}

		// Enemie spawning setup
		var spawnerScene = GD.Load<PackedScene>("res://Scenes/Enemies/SpawnEnemies.tscn");
		if (spawnerScene != null)
		{
			var spawner = spawnerScene.Instantiate<SpawnEnemies>();
			spawner.Player = this;
			GetParent().AddChild(spawner);
		}
		else
		{
			GD.PrintErr("ERROR Failed to load SpawnEnemies-Scene");
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (multiplayerSynchronizer.GetMultiplayerAuthority() != Multiplayer.GetUniqueId())
		{
			return; // player should not move, not in focus / not authority .. dont need to render physics
		}
		camera = GetNode<Camera2D>("Camera2D");
		camera.MakeCurrent(); // enable camera if if player is authority

		Vector2 direction = Vector2.Zero;

		// Check if joystick exists and if it's being used
		if (Joystick != null)
		{
			Vector2 joystickDirection = (Vector2)Joystick.Get("PosVector");
			if (joystickDirection != Vector2.Zero)
			{
				direction = joystickDirection;
			}
		}


		// If no joystick input, fallback to keyboard
		if (direction == Vector2.Zero)
		{
			direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		}

		Velocity = direction * Speed;
		MoveAndSlide();
	}

		private void DebugIt(string message)
	{
		if (enableDebug)
		{
			Debug.Print(message);
		}
	}
}
