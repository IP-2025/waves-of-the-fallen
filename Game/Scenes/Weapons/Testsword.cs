using Godot;
using System;

public partial class Testsword : MeleeWeapon
{
	private Sprite2D daggerSprite;
	public override void _Ready()
	{
		GD.Print("Sword scene loaded!");
		daggerSprite = GetNode<Sprite2D>("Sprite2D");
	}
}
