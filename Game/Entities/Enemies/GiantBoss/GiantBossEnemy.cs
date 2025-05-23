using Godot;
using System;
using System.Diagnostics;
using System.Numerics;


public partial class GiantBossEnemy : EnemyBase
{
	[Export] public PackedScene StoneAttackScene;
	public GiantBossEnemy()
	{
		speed = 70f;
		damage = 15f;
		attacksPerSecond = 1f;
	}
	public override void Attack()
	{
		player.GetNode<Health>("Health").Damage(damage);

		if (enableDebug)
		{
			Debug.Print($"Giant Boss attacks (melee) with speed: {speed}, damage: {damage}, attacksPerSecond: {attacksPerSecond}!");
		}
		DoSpecialAttack();
    }

    private void DoSpecialAttack()
    {
		int random =GD.RandRange(1, 2);
        if (StoneAttackScene != null && player != null && random == 1)
        {
            if (enableDebug)
            {
                GD.Print($"GiantBossEnemy: {Name} is attacking with his special attack.");
            }

            Godot.Vector2 startDirection = (player.GlobalPosition - GlobalPosition).Normalized();
            float angle = Mathf.DegToRad(360 / 5);

            Godot.Vector2[] directions = new Godot.Vector2[5];
            directions[0] = startDirection;
            directions[1] = startDirection.Rotated(angle);
            directions[2] = startDirection.Rotated(2 * angle);
            directions[3] = startDirection.Rotated(3 * angle);
            directions[4] = startDirection.Rotated(4 * angle);
            foreach (Godot.Vector2 direction in directions)
            {
				Callable.From(() => SpawnStones(direction)).CallDeferred();
            }

        }
    }

    private void SpawnStones(Godot.Vector2 direction)
    {
        var projectile = (StoneAttack)StoneAttackScene.Instantiate();
        GetParent().AddChild(projectile);
        projectile.GlobalPosition = GlobalPosition;
        projectile.Initialize(direction, damage * 0.5f);
        if (enableDebug)
        {
            GD.Print($"{Name} fires projectile (Stone). Direction: {direction} Damage: {damage * 0.5f}");
        }
    }


    public  void OnAttackRangeBodyEnterSpecialAttack(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			DoSpecialAttack();
			if (enableDebug)
			{
				GD.Print("GiantBossEnemy: Player special attack entered range.");
			}
		}
	}

	public  void OnAttackRangeBodyExitSpecialAttack(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			if (enableDebug)
			{
				GD.Print("GiantBossEnemy: Player special attack entered range.");
			}
		}
	}

	
}
