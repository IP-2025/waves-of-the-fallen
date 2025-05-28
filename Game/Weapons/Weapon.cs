
using Godot;

public abstract partial class Weapon : Area2D
{
    public abstract string IconPath { get; }
    public abstract float ShootDelay { get; set; }

    public abstract float DefaultRange { get; set;}
    public abstract int DefaultDamage { get; set;}
    public abstract int DefaultPiercing { get; set;}
    public abstract float DefaultSpeed { get; set;}
    
    
    public (int dmg, float range, int piercing, float speed, float delay) BaseStats() =>
        (DefaultDamage, DefaultRange, DefaultPiercing, DefaultSpeed, ShootDelay);
    
}