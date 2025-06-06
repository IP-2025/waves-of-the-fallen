// FloatingScore.cs
using Godot;

public partial class FloatingScore : Label
{
    private float _lifetime = 1.0f;
    private float _elapsed = 0f;
    private Vector2 _velocity = new Vector2(0, -60);
    private float _startScale = 0.7f;
    private float _endScale = 1.3f;

    public override void _Ready()
    {
        Scale = new Vector2(_startScale, _startScale);
        Modulate = new Color(1, 1, 0, 1); // yellow
    }

    public override void _Process(double delta)
    {
        _elapsed += (float)delta;
        Position += _velocity * (float)delta;
        float t = Mathf.Clamp(_elapsed / _lifetime, 0, 1);
        float scale = Mathf.Lerp(_startScale, _endScale, t);
        Scale = new Vector2(scale, scale);
        Modulate = new Color(1, 1, 0, 1 - t);

        if (_elapsed >= _lifetime)
            QueueFree();
    }
}