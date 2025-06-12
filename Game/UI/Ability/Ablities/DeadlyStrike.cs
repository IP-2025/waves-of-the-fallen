using Godot;
using System;

public partial class DeadlyStrike : AbilityBase
{
    private int cooldown = 30;
    public override int getCooldown()
    {
        return cooldown;
    }

}
