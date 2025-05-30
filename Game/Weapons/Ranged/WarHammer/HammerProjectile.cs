using Godot;
using Godot.Collections;

public partial class HammerProjectile : Projectile
{
	protected float Radius = 100;

	private bool hasHit = false;

	public override void _Ready()
	{
		Speed = 800;
		Damage = 120;
	}
	public override void OnBodyEntered(Node2D body)
	{
		Speed = 0;
		SetDeferred("Monitoring", false);
		GetNode<AnimatedSprite2D>("./HammerProjectile").Hide();
		GetNode<AnimatedSprite2D>("./Cracks").Play("cracks");
		if (!hasHit) {
			hasHit = true;
			DamageProcess();
		}
	}

	private void DamageProcess() {
		Array<Node> enemyListSnapshot = GetTree().GetNodesInGroup("enemies"); // this is to prevent some funky on enemy interactions with enemies spawned during this check
		foreach (Node node in enemyListSnapshot) // checks if enemies are in attack radius
		{
			if (node is not EnemyBase enemyNode)
				continue;

			float dist = GlobalPosition.DistanceTo(enemyNode.GlobalPosition);
			var healthNode = enemyNode.GetNodeOrNull<Health>("Health");

			if (dist < Radius && healthNode != null)
				healthNode.Damage(Damage);
		}
	}

	private void OnCracksAnimationFinished() {
		QueueFree();
	}

}
