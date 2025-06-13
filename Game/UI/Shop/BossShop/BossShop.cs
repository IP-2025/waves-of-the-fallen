using Godot;
using System.Collections.Generic;
using System.Diagnostics;

public partial class BossShop : Control
{
	[Signal] 
	public delegate void WeaponChosenEventHandler(Weapon weapon);

	private static readonly List<Weapon> allWeapons = new()
{
	new Bow(),
	new Crossbow(),
	new Kunai(),
	new FireStaff(),
	new Lightningstaff(),
	new Healstaff(),
	new Dagger(),
	new Sword(),
	new WarHammer(),
	new DoubleBlade(),
	new MedicineBag()
};

	private void PopulateWeapon(string containerPath, Weapon weapon, int index)
	{
		var box = GetNode<Node>(containerPath);

		box.GetNode<TextureRect>("texture").Texture = GD.Load<Texture2D>(weapon.IconPath);

		var stats = weapon.BaseStats();

		box.GetNode<RichTextLabel>("name").Text = weapon.GetType().Name;
		box.GetNode<RichTextLabel>("damage").Text = weapon is Healstaff || weapon is MedicineBag ? $"Heal:   {stats.dmg}" : $"Damage:   {stats.dmg}";
		box.GetNode<RichTextLabel>("range").Text = $"Range:    {(int)stats.range}";
		box.GetNode<RichTextLabel>("delay").Text = $"Attacks/s:    {stats.delay}";
		box.GetNode<RichTextLabel>("piercing").Text = weapon is RangedWeapon ? $"Piercing: {stats.piercing}" : "";
		
		var btn = GetNode<Button>(containerPath);
		var w = weapon; // closure copy
		btn.Pressed += () => OnWeaponButtonUp(weapon);
		
	}

	public override void _Ready()
	{
		var rng = new RandomNumberGenerator();
		rng.Randomize();

		var shuffled = new List<Weapon>(allWeapons);
		for (int i = shuffled.Count - 1; i > 0; i--)
		{
			int j = rng.RandiRange(0, i);
			(shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
		}

		for (int slot = 0; slot < 3 && slot < shuffled.Count; slot++)
		{
			string path = $"MarginContainer/HBoxContainer/weapon{slot + 1}";
			PopulateWeapon(path, shuffled[slot], slot + 1);
		}
	}

	public void OnWeaponButtonUp(Weapon chosen)
	{
		Debug.Print($"Du hast gewählt: {chosen.GetType().Name}");
		EmitSignal(SignalName.WeaponChosen, chosen);
	}
}
