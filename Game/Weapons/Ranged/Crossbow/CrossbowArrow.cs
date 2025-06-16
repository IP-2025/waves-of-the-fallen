public partial class CrossbowArrow : Projectile
{
	public const float DefaultSpeed    = 800f;
	public const int   DefaultDamage   = 100;
	public const int   DefaultPiercing = 3;

	public override void _Ready()
	{

		int dexdummy = 100;
		int strdummy = 100;
		int intdummy = 100;
		
		Speed    = DefaultSpeed;
		Damage   = DefaultDamage + (int)(dexdummy/1.2f + strdummy + intdummy/5)/3;
		Piercing = DefaultPiercing;
	}
}
