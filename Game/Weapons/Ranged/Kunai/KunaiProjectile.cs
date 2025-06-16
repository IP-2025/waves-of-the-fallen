public partial class KunaiProjectile : Projectile
{
	public const float DefaultSpeed = 600f;
	public const int DefaultDamage = 40;
	public const int DefaultPiercing = 1;
	public override void _Ready()
	{
		int dexdummy = 100;
		int strdummy = 100;
		int intdummy = 100;

		Speed    = DefaultSpeed + (int)(dexdummy + strdummy/8 + intdummy/8)/3;
		Damage   = DefaultDamage;
		Piercing = DefaultPiercing;
	}
}
