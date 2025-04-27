using Godot;


public partial class MapLoader : Node {
    public void LoadMap(string path) {
        var scene = GD.Load<PackedScene>(path);
        var map = scene.Instantiate<Node2D>();
        GetTree().Root.AddChild(map);
    }
}