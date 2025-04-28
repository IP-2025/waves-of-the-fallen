using Godot;
using System;

public partial class ButtonsCharacterSelection : Button
{
	private Charactermenu _controller;
	private Shader _blackAndWhiteShader;
	
	 public override void _Ready()
	{
		if(_blackAndWhiteShader==null)
		{
			_blackAndWhiteShader=GD.Load<Shader>("res://Scenes/Menu/characterMenuIconShader.gdshader");
		}
		Node node=GetParent();
		while (_controller==null&&node!=null){
			if(node is Charactermenu)
			{
				_controller=(Charactermenu)node;
				break;
			}
			node=node.GetParent();
		}
		
		if(this.Name=="Button_Character6"||this.Name=="Button_Character3") //if Character is locked =true
		{
			var icon = GetNode<TextureRect>("TextureRect");
			var material=new ShaderMaterial();
			material.Shader=_blackAndWhiteShader;
			icon.Material= material;
		}
		
	}
	
	private void _on_pressed()
	{
		_controller._characterSelected(this);
	}
}
