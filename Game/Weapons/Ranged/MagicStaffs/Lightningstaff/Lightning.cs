using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
public partial class Lightning : Projectile
{
	public const float DefaultSpeed = 1000f;
	public const int DefaultDamage = 60;
	public const int DefaultPiercing = 1;
	
	private PackedScene _lightningScene = GD.Load<PackedScene>("res://Weapons/Ranged/MagicStaffs/Lightningstaff/lightning.tscn");
	protected float Radius = 200;
	protected int Jumps = DefaultPiercing;

	private bool _hasHit = false;
	private Node2D _ignoredBody = null;

	public override void _Ready()
    {
        		DefaultPlayer OwnerNode = GetNode("../../").GetParentOrNull<DefaultPlayer>();
        int dex = OwnerNode.Dexterity;
        int str = OwnerNode.Strength;
        int @int = OwnerNode.Intelligence;

        Speed = 1000;
        Damage = 60 + (int)(dex / 3.5f + str / 7 + @int) / 3;
        Jumps = Jumps + Math.Max((int)((@int - 100) / 50f), 0);
    }

    public override void OnBodyEntered(Node2D body)
	{
		if (body == _ignoredBody)
			return;
		Speed = 0;
		SetDeferred("Monitoring", false);
		GetNode<AnimatedSprite2D>("./LightningAnimation").Hide();
		GetNode<AnimatedSprite2D>("./Static").Play("static");


		if (!_hasHit)
		{
			_hasHit = true;
			DamageProcess(body);
		}
	}

	private void DamageProcess(Node2D body)
	{
		var healthNode = body.GetNodeOrNull<Health>("Health");
		if (healthNode != null)
		{
			healthNode.Damage(Damage);
		}

		if (Jumps > 0)
		{
			var targets = GetNearestEnemies(3);

			if (targets.Count > 0)
				targets.RemoveAt(0);

			foreach (EnemyBase target in targets)
			{
				var lightning = _lightningScene.Instantiate<Lightning>();
				lightning.GlobalPosition = GlobalPosition;

				lightning._ignoredBody = body;
				lightning.Jumps = Jumps - 1;

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

	private void OnStaticAnimationFinished()
	{
		QueueFree();
	}
}
