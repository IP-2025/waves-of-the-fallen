using Godot;
using System;
using System.Collections.Generic;
using WeaponDataShop;

public partial class BossShop : Control
{
	private List<WeaponData> weaponData = new()
	{
		new WeaponData("Langschwert", "+ Schaden", "- Langsam", "res://UI/Shop/BossShop/schwert1.webp"),
		new WeaponData("Eisschwert", "+ Verlangsamt Gegner", "- Schaden", "res://UI/Shop/BossShop/schwert1.webp"),
		new WeaponData("Dolch", "+ Schnell", "- Geringer Schaden", "res://UI/Shop/BossShop/schwert1.webp"),
		new WeaponData("Hammer", "+ Großer Schaden", "- Sehr langsam", "res://UI/Shop/BossShop/schwert1.webp"),
		new WeaponData("Bogen", "+ Fernkampf", "- Muss gespannt werden", "res://UI/Shop/BossShop/schwert1.webp")
	};

	private string[] weaponNodes = { "weapon1", "weapon2", "weapon3" };

	public override void _Ready()
	{
		var random = new Random();
		var shuffled = new List<WeaponData>(weaponData);
		shuffled.Sort((a, b) => random.Next(-1, 2));

		var container = GetNode("MarginContainer/HBoxContainer");

		for (int i = 0; i < 3; i++)
		{
			var data = shuffled[i];
			var node = container.GetNode<Control>(weaponNodes[i]);

			var textureRect = node.GetNode<TextureRect>("texture");
			textureRect.Texture = GD.Load<Texture2D>(data.ImagePath);

			var nameLabel = node.GetNode<RichTextLabel>("name");
			nameLabel.Text = data.Name;

			var advLabel = node.GetNode<RichTextLabel>("advantage");
			advLabel.Text = data.Advantage;

			var disadvLabel = node.GetNode<RichTextLabel>("disadvantage");
			disadvLabel.Text = data.Disadvantage;
		}
	}
}
