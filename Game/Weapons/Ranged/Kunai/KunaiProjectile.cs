public partial class KunaiProjectile : Projectile
{
	public const float DefaultSpeed = 600f;
	public const int DefaultDamage = 40;
	public const int DefaultPiercing = 1;
	public override void _Ready()
	{
		Speed    = DefaultSpeed + (int)(dex + str/8 + @int/8)/3;
		Damage   = DefaultDamage;
		Piercing = DefaultPiercing;
	}
}
