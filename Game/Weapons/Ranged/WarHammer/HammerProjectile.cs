using System;
using Godot;
using Godot.Collections;

public partial class HammerProjectile : Projectile
{
	
	public const float DefaultSpeed = 800f;
	public const int DefaultDamage = 120;
	public const int DefaultPiercing = 1;
	
	public float Radius = 100;

	private bool hasHit = false;

	public override void _Ready()
	{
		int dexdummy = 100;
		int strdummy = 100;
		int intdummy = 100;

		Speed = DefaultSpeed;
		Damage = DefaultDamage + (int)(dexdummy / 5 + strdummy + intdummy / 7.5f) / 3;
		Piercing = DefaultPiercing;
		Radius = Radius + Math.Max(Math.Min((strdummy-100f)/2f,50f),0); 
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
