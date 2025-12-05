using Godot;
using System;

public partial class Door : StaticBody3D
{
    [Export] public float WaitTime = 5;
    [Export] public MeshInstance3D mesh;
    [Export] public CollisionShape3D coll;
    [Export] public ShaderMaterial mat;
    public AudioStreamPlayer3D SFX;
    [Export] AudioStream DE;
    [Export] AudioStream DD;
    public Tween tween = null;
    public override void _Ready()
    {
        mesh.MaterialOverride = mat;
        SFX = new AudioStreamPlayer3D();
        AddChild(SFX);
        end();
    }
    public void Init()
    {
        if (tween == null || !tween.IsRunning())
        {
            coll.Disabled = true;
            tween = CreateTween();
            mat.SetShaderParameter("opacity", 0.0f);
            SFX.Stream = DD;
            SFX.Play();
            tween.TweenProperty(mat, "shader_parameter/opacity", 1.0f, WaitTime);
            tween.TweenCallback(Callable.From(endd));
        }
    }
    public void end()
    {
        if (tween != null) tween.Stop();
        mat.SetShaderParameter("opacity", 1.0f);
        coll.Disabled = false;
        tween = null;
    }
     public void endd()
    {
        SFX.Stream = DE;
        SFX.Play();
        if (tween!=null) tween.Stop();
        mat.SetShaderParameter("opacity",1.0f);
        coll.Disabled = false;
        tween = null;
    }
}
