using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

public partial class BossShop : Control{
	
private static readonly List<RangedWeapon> allWeapons = new()
{
	new Bow(),
	new Crossbow(),
	new Kunai(),
	new FireStaff(),
	new Lightningstaff()
};

private void PopulateWeapon(string containerPath, RangedWeapon weapon)
{
	var box = GetNode<Node>(containerPath);

	box.GetNode<TextureRect>("texture").Texture = GD.Load<Texture2D>(weapon.IconPath);

	var stats = weapon.BaseStats();

	box.GetNode<RichTextLabel>("name").Text     = weapon.GetType().Name;
	box.GetNode<RichTextLabel>("damage").Text   = $"Damage:   {stats.dmg}";
	box.GetNode<RichTextLabel>("range").Text    = $"Range:    {(int)stats.range}";
	box.GetNode<RichTextLabel>("piercing").Text = $"Piercing: {stats.piercing}";
	box.GetNode<RichTextLabel>("speed").Text    = $"Speed:    {(int)stats.speed}";
}

public override void _Ready()
{
	var rng = new RandomNumberGenerator();
	rng.Randomize();

	var shuffled = new List<RangedWeapon>(allWeapons);
	for (int i = shuffled.Count - 1; i > 0; i--)
	{
		int j = rng.RandiRange(0, i);
		(shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
	}

	for (int slot = 0; slot < 3 && slot < shuffled.Count; slot++)
	{
		string path = $"MarginContainer/HBoxContainer/weapon{slot + 1}";
		PopulateWeapon(path, shuffled[slot]);
	}
}
}
