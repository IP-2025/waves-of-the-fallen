using Godot;
using System;

public partial class JoyHandle : Sprite2D
{
    private Node2D _parent;
    private bool _pressing = false;

    [Export]
    public float MaxLength = 45.0f;

    private float _deadzone = 15.0f;

    public override void _Ready()
    {
        _parent = GetParent<Node2D>();
        
        if (_parent is Joystick joystick)
        {
            _deadzone = joystick.Deadzone;
            MaxLength *= _parent.Scale.X;
        }
    }

    public override void _Process(double delta)
    {
        if (_pressing)
        {
            Vector2 parentGlobalPos = _parent.GlobalPosition;
            Vector2 mousePos = GetGlobalMousePosition();

            if (mousePos.DistanceTo(parentGlobalPos) <= MaxLength)
            {
                GlobalPosition = mousePos;
            }
            else
            {
                float angle = parentGlobalPos.AngleToPoint(mousePos);
                GlobalPosition = parentGlobalPos + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * MaxLength;
            }

            CalculateVector();
        }
        else
        {
            GlobalPosition = GlobalPosition.Lerp(_parent.GlobalPosition, (float)(delta * 50));
            SetJoystickPosVector(Vector2.Zero);
        }
    }

    private void CalculateVector()
    {
        Vector2 parentGlobalPos = _parent.GlobalPosition;
        Vector2 diff = GlobalPosition - parentGlobalPos;

        Vector2 posVector = Vector2.Zero;

        if (Mathf.Abs(diff.X) >= _deadzone)
            posVector.X = diff.X / MaxLength;
        
        if (Mathf.Abs(diff.Y) >= _deadzone)
            posVector.Y = diff.Y / MaxLength;

        SetJoystickPosVector(posVector);
    }

    private void SetJoystickPosVector(Vector2 posVector)
    {
        if (_parent is Joystick joystick)
        {
            joystick.PosVector = posVector;
        }
    }

    private void OnButtonDown()
    {
        _pressing = true;
    }

    private void OnButtonUp()
    {
        _pressing = false;
    }
}
