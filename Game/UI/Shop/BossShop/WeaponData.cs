namespace WeaponDataShop
{
public class WeaponData
{
	public string Name;
	public string Advantage;
	public string Disadvantage;
	public string ImagePath;

	public WeaponData(string name, string advantage, string disadvantage, string imagePath)
	{
		Name = name;
		Advantage = advantage;
		Disadvantage = disadvantage;
		ImagePath = imagePath;
	}
}
}
