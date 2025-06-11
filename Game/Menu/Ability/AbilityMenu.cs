using Godot;
using System;

public partial class AbilityMenu : Control
{
	private void _on_button_back_abilitymenu_pressed()
    {
        var scene = ResourceLoader.Load<PackedScene>("res://Menu/Character/characterMenu.tscn");
        SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
        GetTree().ChangeSceneToPacked(scene);
    }
}
