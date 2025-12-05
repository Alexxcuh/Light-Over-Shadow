using Godot;
using System;

public partial class FadeText : Label3D
{
    [Export] public float distance = 6.7f;
    public override void _PhysicsProcess(double delta)
    {
        Color c = Modulate;
        c.A = Mathf.Clamp(1f - (Position.DistanceTo(GetTree().Root.GetCamera3D().GlobalPosition) / distance),0.0f,1.0f);
        Modulate = c;
        OutlineModulate = new Color(0, 0, 0, c.A);
    }
}
