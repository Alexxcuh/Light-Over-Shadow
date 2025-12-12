using Godot;
using System;

public partial class scroll : Panel
{
    [Export] Label BaseLabel;
    Label Label2;
    public override void _Ready()
    {
        Label2 = (Label)BaseLabel.Duplicate();
        Label2.Position = new Vector2(BaseLabel.Size.X,0);
        AddChild(Label2);
        base._Ready();
    }
    public override void _PhysicsProcess(double delta)
    {
        Scroll(delta,150f);

        base._PhysicsProcess(delta);
    }
    public void Scroll(double delta, float speed)
    {
        Vector2 l1Pos = BaseLabel.Position;
        Vector2 l2Pos = Label2.Position;
        float labelWidth = BaseLabel.Size.X;
        float moveAmount = (float)delta * speed;

        // Move both labels
        l1Pos.X -= moveAmount;
        l2Pos.X -= moveAmount;


        if (l1Pos.X <= -labelWidth)
            l1Pos.X = l2Pos.X + labelWidth;

        if (l2Pos.X <= -labelWidth)
            l2Pos.X = l1Pos.X + labelWidth;

        BaseLabel.Position = l1Pos;
        Label2.Position = l2Pos;
    }

}
