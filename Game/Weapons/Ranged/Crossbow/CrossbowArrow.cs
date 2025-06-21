using System;

public partial class CrossbowArrow : Projectile
{
	public const float DefaultSpeed    = 800f;
	public const int   DefaultDamage   = 100;
	public const int   DefaultPiercing = 3;

	public override void _Ready()
	{	
		Speed    = DefaultSpeed;
		Damage   = DefaultDamage + (int)(dex/1.2f + str + @int/5)/3;
		Piercing = DefaultPiercing + Math.Max((int)((str - 100) / 50f), 0);
	}
}
