using System.Threading.Tasks;
using Godot;

public partial class Medicine : Area2D
{

	AnimatedSprite2D medicineSprite;

	private Vector2 endPos;
	protected int healValue = 10;

	public override void _Ready()
	{
		medicineSprite = GetNode<AnimatedSprite2D>("MedicineSprite");
		PathFollow2D path = GetNode<PathFollow2D>("Path2D/PathFollow2D");
		path.ProgressRatio = GD.Randf();
		endPos = path.GlobalPosition;
		_ = EnablePlayerCollision();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (GlobalPosition.DistanceTo(endPos) > 10)
			GlobalPosition += (endPos - GlobalPosition) / 2 * (float)delta;
	}

	private async Task EnablePlayerCollision()
	{
		await ToSignal(GetTree().CreateTimer(0.7), "timeout");
		medicineSprite.Play("thrown");
		SetCollisionMaskValue(1, true);
	}


	private void OnBodyEntered(Node2D body)
	{
		var healthNode = body.GetNodeOrNull<Health>("Health");
		if (healthNode != null)
		{
			healthNode.Heal(healValue);
			SoundManager.Instance.PlaySoundAtPosition(SoundManager.Instance.GetNode<AudioStreamPlayer2D>("healItemPickUp"), GlobalPosition, -10);
		}
		QueueFree();
	}

	private void OnDespawnTimeTimeout()
	{
		_ = Despawn();
	}

	private async Task Despawn()
	{
		medicineSprite.Play("despawning");
		await ToSignal(GetTree().CreateTimer(3), "timeout");
		QueueFree();
	}
}
