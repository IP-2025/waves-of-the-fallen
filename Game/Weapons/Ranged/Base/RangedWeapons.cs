using Godot;

public abstract partial class RangedWeapon : Weapon
{
	protected AnimatedSprite2D animatedSprite;
	protected PackedScene projectileScene;

	public abstract string ResourcePath { get; }

	public abstract int SoundFrame { get; }
	

	public override void _PhysicsProcess(double delta)
	{
		var target = FindNearestEnemy();
		if (target != null && TryGetPosition(target, out var position))
			LookAt(position);
	}

	protected Node FindNearestEnemy()
	{
		float closestDist = float.MaxValue;
		Node closestEnemy = null;

		foreach (Node node in GetTree().GetNodesInGroup("enemies"))
		{
			if (node is not EnemyBase enemyNode)
				continue;

			float dist = GlobalPosition.DistanceTo(enemyNode.GlobalPosition);

			if (dist < closestDist && dist <= DefaultRange)
			{
				closestDist = dist;
				closestEnemy = enemyNode;
			}
		}
		return closestEnemy;
	}

	protected bool TryGetPosition(object body, out Vector2 position)
	{
		position = Vector2.Zero;
		if (body is GodotObject obj)
		{
			Variant globalPosVariant = obj.Get("global_position");
			if (globalPosVariant.VariantType == Variant.Type.Vector2)
			{
				position = (Vector2)globalPosVariant;
				return true;
			}
			Variant posVariant = obj.Get("position");
			if (posVariant.VariantType == Variant.Type.Vector2)
			{
				position = (Vector2)posVariant;
				return true;
			}
		}
		return false;
	}

	protected void Shoot()
	{
		if (projectileScene == null) return;

		var projectileInstance = projectileScene.Instantiate<Area2D>();
		var shootingPoint = GetNode<Marker2D>("ShootingPoint");

		projectileInstance.GlobalPosition = shootingPoint.GlobalPosition;
		projectileInstance.GlobalRotation = shootingPoint.GlobalRotation;

		GetTree().CurrentScene.AddChild(projectileInstance);
	}
}
