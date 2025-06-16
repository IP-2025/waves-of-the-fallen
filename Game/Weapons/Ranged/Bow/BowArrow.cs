public partial class BowArrow : Projectile
{
	public const float DefaultSpeed = 1200f;
	public const int DefaultDamage = 50;
	public const int DefaultPiercing = 1;

	public override void _Ready()
	{

		int dexdummy = 100;
		int strdummy = 100;
		int intdummy = 100;
		
		Speed    = DefaultSpeed;
		Damage   = DefaultDamage + (int)(dexdummy + strdummy/3.5f + intdummy/7)/3;
		Piercing = DefaultPiercing;
	}
}
