using Godot;

public partial class MountedEnemy : EnemyBase
{
	[Export] public float stopDistance = 200f; // distance at which MountedEnemy stops moving

	public override void _Ready()
	{
		base._Ready();

		// connect health depleted signal
		var health = GetNode<Health>("Health");
		if (health != null)
		{
			health.HealthDepleted += OnHealthDepleted;
		}

		// check if sprite texture is assigned
		var sprite = GetNode<Sprite2D>("Sprite2D");
		if (sprite.Texture == null)
		{
			GD.PrintErr("Texture is not assigned to Sprite2D in MountedEnemy!");
		}
		else
		{
			GD.Print("Texture is assigned correctly to Sprite2D in MountedEnemy.");
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		FindNearestPlayer(); // find the closest player
		if (player == null)
		{
			GD.Print("No player found. MountedEnemy is idle."); // debug message
			Velocity = Vector2.Zero; // stop moving if no player is found
			MoveAndSlide();
			return;
		}

		float dist = GlobalPosition.DistanceTo(player.GlobalPosition); // calculate distance to player
		GD.Print($"Distance to player: {dist}"); // debug message

		LookAt(player.GlobalPosition); // face the player

		if (dist > stopDistance)
		{
			// move towards the player
			Vector2 toPlayer = (player.GlobalPosition - GlobalPosition).Normalized();
			Velocity = toPlayer * 400f; // fixed speed for MountedEnemy
			GD.Print("MountedEnemy is moving towards the player."); // debug message
		}
		else
		{
			Velocity = Vector2.Zero; // stop moving when within stop distance
			GD.Print("MountedEnemy is within stop distance and has stopped."); // debug message
		}

		MoveAndSlide(); // apply movement
	}

	public override void Attack()
	{
		GD.Print("MountedEnemy attacks!"); // debug message for attack
	}

	private void OnHealthDepleted()
	{
		GD.Print("MountedEnemy health depleted. Spawning Rider..."); // debug message
		ActivateRider(); // spawn Rider when health is depleted
	}

	private void ActivateRider()
	{
		GD.Print("Attempting to spawn Rider..."); 

		// load Rider scene
		PackedScene riderScene = GD.Load<PackedScene>("res://Scenes/Characters/rider_enemy.tscn");
		if (riderScene == null)
		{
			GD.PrintErr("Failed to load Rider scene!"); 
			return;
		}

		// instantiate Rider
		var riderInstance = riderScene.Instantiate<CharacterBody2D>();
		if (riderInstance == null)
		{
			GD.PrintErr("Failed to instantiate Rider!"); 
			return;
		}

		// set Rider position and add to scene
		riderInstance.GlobalPosition = GlobalPosition; // Rider spawns at MountedEnemy's position
		riderInstance.Visible = true; // make Rider visible
		GetParent().AddChild(riderInstance); // add Rider to scene
		GD.Print($"Rider successfully spawned at position {riderInstance.GlobalPosition}."); // debug message

		// debug: check Rider's children
		foreach (Node child in riderInstance.GetChildren())
		{
			GD.Print($"Child node: {child.Name}, Type: {child.GetType()}, Visible: {child is CanvasItem canvasItem && canvasItem.Visible}");
		}

		// check if Rider was added to the scene
		if (riderInstance.GetParent() == null)
		{
			GD.PrintErr("Rider was not added to the scene!"); // error message
		}
		else
		{
			GD.Print($"Rider parent: {riderInstance.GetParent().Name}"); // debug message
		}
	}
}
