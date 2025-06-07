using Godot;
using System;

public partial class Shield : Sprite2D
{
    private Node2D _parent;
    private Node2D health;
    public override void _Ready()
    {
        _parent = GetParent<Node2D>();
        health = _parent.GetNode<Health>("Health");
    }
}
