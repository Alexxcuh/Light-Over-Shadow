using Godot;

public partial class LightPlatform : StaticBody3D
{
    public bool enabled = false;
    [Export] public float LifeTime;
    private Timer Life;
    [Export] StandardMaterial3D mat;
    private StandardMaterial3D material;
    [Export] MeshInstance3D mesh;
    [Export] AudioStreamPlayer3D SFX;
    [Export] AudioStream Place;
    [Export] AudioStream Dissapear;
    public Player plr;
    public void Init()
    {
        if (!enabled) return;
        SFX.Stream = Place;
        SFX.Play();
        Life = new Timer();
        Life.WaitTime = LifeTime;
        Tween tween = GetTree().CreateTween();
        material = (StandardMaterial3D)mat.Duplicate();
        tween.TweenProperty(material, "albedo_color", Colors.Black, 5.0f);
        tween.TweenProperty(this, "scale", Vector3.One * 0.01f, 0.05f);
        mesh.MaterialOverride = material;
        tween.TweenCallback(Callable.From(Begone));
    }
    public void Begone()
    {
        Visible = false;
        enabled = false;
        SFX.Stream = Dissapear;
        SFX.Play();
    }
    public void finish()
    {
        if (Visible == false && enabled == false)
        {
            QueueFree();
        }
    }
}
