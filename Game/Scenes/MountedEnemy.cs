using Godot;
using System;

public partial class MountedEnemy : Node2D
{
	[Export] public PackedScene RiderScene;
	[Export] public PackedScene MountScene;

	private Node2D _rider;
	private Node2D _mount;

	public override void _Ready()
	{
		// Instance the rider and mount
		_mount = (Node2D)MountScene.Instantiate();
		_rider = (Node2D)RiderScene.Instantiate();

		// Add them as children
		AddChild(_mount);
		AddChild(_rider);

		// Position the rider on the mount
		_rider.Position = _mount.Position + new Vector2(0, -20); // Adjust offset as needed

		// Connect the mount's defeated signal
		if (_mount is Mount mountScript)
		{
			mountScript.Defeated += OnMountDefeated;
		}
	}

	private void OnMountDefeated()
	{
		// Handle dismounting logic
		RemoveChild(_mount);
		_rider.Position = Position; // Place rider at the current position
		AddChild(_rider);
	}
}
