using Godot;

public partial class rotatee : Label3D
{
    private float vel = 670f;
    private float ry;
    private float rx;
    [Export] Label3D BackSide;

    public override void _Ready()
    {
        if (LOSGlobals.Experimental) BackSide.Text = "EXPERIMENTAL";
        ry = 1 - (GD.Randf() * 2f); 
        rx = 1 - (GD.Randf() * 2f);
    }

    public override void _PhysicsProcess(double delta)
    {
        vel *= 0.96f;
        RotateY(ry * 0.025f  * (1+vel));
        RotateZ(rx * 0.025f * (1+vel));
    }
}
