using Godot;
using System;

public partial class DefaultPlayer : CharacterBody2D
{
	[Export]
	public float Speed = 600.0f;
	private Node2D _joystick;
	private Camera2D camera;

    public override void _Ready()
    {
		// Find the joystick
		_joystick = GetNode<Node2D>("./Joystick");

        // set MultiplayerSyncronizer as authority of this id and then check authority against this id to see if player is authority
        // to later enable movement controlls and so on for the current player only
        var multiplayerSynchronizer = GetNodeOrNull<MultiplayerSynchronizer>(
            "MultiplayerSynchronizer"
        );
        if (multiplayerSynchronizer != null)
        {
			GD.Print("Trying to parse Name as int: ", Name);
            multiplayerSynchronizer.SetMultiplayerAuthority(int.Parse(Name));
        }
        else
        {
            GD.PrintErr("Multiplayer Node was not found");
        }
    }

	public override void _PhysicsProcess(double delta)
	{
		if (
            GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority()
            != Multiplayer.GetUniqueId()
        )
        {
            return; // player should not move, not in focus / not authority .. dont need to render physics
        }
        camera = GetNode<Camera2D>("Camera2D");
        camera.MakeCurrent(); // enable camera if if player is authority

		Vector2 direction = Vector2.Zero;

		// Check if joystick exists and if it's being used
		if (_joystick != null)
		{
			Vector2 joystickDirection = (Vector2)_joystick.Get("PosVector");
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
	
}
