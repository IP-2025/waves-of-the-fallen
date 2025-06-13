using Godot;
using System;
using Game.Menu.Character;

public partial class ButtonsCharacterSelection : Button
{
	private Charactermenu _controller;
	private Shader _blackAndWhiteShader;
	private CharacterManager characterManager;

	public override void _Ready()
	{
		characterManager = GetNode<CharacterManager>("/root/CharacterManager");
		if (_blackAndWhiteShader == null)
		{
			_blackAndWhiteShader = GD.Load<Shader>("res://Menu/Character/characterMenuIconShader.gdshader");
		}
		Node node = GetParent();
		while (_controller == null && node != null)
		{
			if (node is Charactermenu)
			{
				_controller = (Charactermenu)node;
				break;
			}
			node = node.GetParent();
		}

		if (!characterManager.LoadIsUnlocked(this.Text)) //if Character is locked =true
		{
			var icon = GetNode<TextureRect>("TextureRect");
			var material = new ShaderMaterial();
			material.Shader = _blackAndWhiteShader;
			icon.Material = material;
		}

	}

	public void _on_pressed()
	{
		_controller._characterSelected(this);
		SoundManager.Instance.PlaySound(SoundManager.Instance.GetNode<AudioStreamPlayer>("buttonPress"));
	}
}
