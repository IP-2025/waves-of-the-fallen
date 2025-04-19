using Godot;
using System;

public partial class ButtonsCharacterSelection : Button
{
	private Charactermenu _controller;
	
	 public override void _Ready()
	{
		Node node=GetParent();
		while (_controller==null&&node!=null){
			if(node is Charactermenu)
			{
				_controller=(Charactermenu)node;
				break;
			}
			node=node.GetParent();
		}
	}
	
	private void _on_pressed()
	{
		_controller._characterSelected(this);
	}
}
