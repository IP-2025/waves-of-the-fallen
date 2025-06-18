public partial class BowArrow : Projectile
{
	public const float DefaultSpeed = 1200f;
	public const int DefaultDamage = 50;
	public const int DefaultPiercing = 1;

	public override void _Ready()
	{

		DefaultPlayer OwnerNode = GetNode("../../").GetParentOrNull<DefaultPlayer>();
		int dex = OwnerNode.Dexterity;
		int str = OwnerNode.Strength;
		int @int = OwnerNode.Intelligence;
		
		Speed    = DefaultSpeed;
		Damage   = DefaultDamage + (int)(dex + str/3.5f + @int/7)/3;
		Piercing = DefaultPiercing;
	}
}
