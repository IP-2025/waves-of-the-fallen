using Godot;

public partial class EnemySpawner : Node {
    private Timer _timer;
    public override void _Ready() {
        _timer = new Timer();
        AddChild(_timer);
        _timer.WaitTime = 5f;
        _timer.OneShot = false;
        _timer.Connect("timeout", Callable.From(() => SpawnEnemy()));
    }
    public void Start() {
        _timer.Start();
    }
    private void SpawnEnemy() {
        var scene = GD.Load<PackedScene>("res://Actors/Enemy.tscn");
        var e = scene.Instantiate<Node2D>();
        GetTree().Root.AddChild(e);
        GameManager.Instance.Entities[GameManager.Instance.GetNextId()] = e;
    }
}