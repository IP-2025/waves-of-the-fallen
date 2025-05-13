using Godot;
using Godot.Collections;

public partial class FireBall : Projectile
{
	protected float Radius = 100;

	public override void _Ready()
	{
		Speed = 600;
		Damage = 120;
	}
	public override void OnBodyEntered(Node2D body)
	{
		Array<Node> enemyListSnapshot = GetTree().GetNodesInGroup("enemies"); // this is to prevent some funky on enemy interactions with enemies spawned during this check
		foreach (Node node in enemyListSnapshot) // checks if enemies are in attack radius
		{
			if (node is not EnemyBase enemyNode)
				continue;

			float dist = GlobalPosition.DistanceTo(enemyNode.GlobalPosition);
			var healthNode = enemyNode.GetNodeOrNull<Health>("Health");

			if (dist < Radius && healthNode != null) healthNode.Damage(Damage);
		}

		// made with ducttape
		GetNode<AnimatedSprite2D>("./FireballAnimation").Hide();
		Speed = 0;
		GetNode<AnimatedSprite2D>("./Explosion").Play("explosion");
	}

	private void OnExplosionAnimationFinished() {
		QueueFree();
	}

}
