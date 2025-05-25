using Godot;

public partial class Medicine : Area2D
{
	protected int Damage = -5;
	private void OnBodyEntered(Node2D body)
	{
		var healthNode = body.GetNodeOrNull<Health>("Health");
		if (healthNode != null)
		{
			healthNode.Damage(Damage);
		}
		QueueFree();
	}

	private void OnDespawnTimeTimeout()
	{
		QueueFree();
	}
}
