using Godot;
using System;

public partial class Button3D : MeshInstance3D
{
    [Export] public Player Player;
    [Export] public Label3D InteractLabel;
    [Export] public MeshInstance3D Button;
    [Export] public AudioStreamPlayer3D SFX;
    [Export] public Door door;
    private Tween tween;
    public override void _PhysicsProcess(double delta)
    {
        if (Position.DistanceTo(Player.Position) <= 3f)
        {
            InteractLabel.Visible = true;
            if (Input.IsActionJustPressed("Interact"))
            {
                tween = CreateTween();
                tween.TweenProperty(Button, "position", new Vector3(0, 0.1f, 0), 0.067f);
                tween.TweenCallback(Callable.From(Pressed));
            }
            if (Input.IsActionJustReleased("Interact"))
            {
                tween.Stop();
                tween = CreateTween();
                tween.TweenProperty(Button, "position", new Vector3(0, 0.2f, 0), 0.067f);
            }
        }
        else
        {
            InteractLabel.Visible = false;
        }
    }
    public void Pressed()
    {
        SFX.Play();
        door.Init();
        Player.Reset += door.end;
    }
}
