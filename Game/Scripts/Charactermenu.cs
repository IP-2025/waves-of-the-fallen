using Godot;
using System;

public partial class Charactermenu : Control
{
		private void _on_button_back_charactermenu_pressed()
{
var scene = ResourceLoader.Load<PackedScene>("res://Scenes/mainmenu.tscn");
GetTree().ChangeSceneToPacked(scene);
}
}
