using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;
public partial class Lightning : Projectile
{
	private PackedScene lightningScene = GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Lightningstaff/lightning.tscn");
	protected float Radius = 200;
	protected int jumps = 1;

	private bool hasHit = false;
	private Node2D ignoredBody = null;

	public override void _Ready()
	{
		Speed = 1000;
		Damage = 60;
	}
	public override void OnBodyEntered(Node2D body)
	{
		if (body == ignoredBody)
			return;
		Speed = 0;
		SetDeferred("Monitoring", false);
		GetNode<AnimatedSprite2D>("./LightningAnimation").Hide();
		GetNode<AnimatedSprite2D>("./Static").Play("static");
		

		if (!hasHit) {
			hasHit = true;
			DamageProcess( body);
		}
	}

	private void DamageProcess(Node2D body) {
		var healthNode = body.GetNodeOrNull<Health>("Health");
		if (healthNode != null)
		{
			healthNode.Damage(Damage);
		}

		if (jumps > 0)
		{
			var targets = GetNearestEnemies(3);
		
			if (targets.Count > 0)
				targets.RemoveAt(0);
		
			foreach (EnemyBase target in targets)
			{
				var lightning = lightningScene.Instantiate<Lightning>();
				lightning.GlobalPosition = GlobalPosition;
			
				lightning.ignoredBody = body;
				lightning.jumps = jumps - 1;
			
				Vector2 direction = (target.GlobalPosition - GlobalPosition).Normalized();
				lightning.Rotation = direction.Angle();

				GetParent().AddChild(lightning);
			}
		}
	}
	
	private List<EnemyBase> GetNearestEnemies(int count)
	{
		var list = new List<(EnemyBase enemy, float dist)>();

		foreach (Node node in GetTree().GetNodesInGroup("enemies"))
		{
			if (node is not EnemyBase enemy)
				continue;

			float d = GlobalPosition.DistanceTo(enemy.GlobalPosition);
			if (d > Radius)
				continue;

			list.Add((enemy, d));
		}
		
		return list
			.OrderBy(t => t.dist)
			.Take(count)
			.Select(t => t.enemy)
			.ToList();
	}
	
	private void OnStaticAnimationFinished() {
		QueueFree();
	}
}
