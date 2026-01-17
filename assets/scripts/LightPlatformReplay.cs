using System;
using Godot;

public partial class LightPlatformReplay : StaticBody3D
{
    public bool enabled = false;
    [Export] Freecam boss;
    public int spawnedontick = 0;
    [Export] public float LifeTime;
    [Export] StandardMaterial3D mat;
    private StandardMaterial3D material;
    [Export] MeshInstance3D mesh;
    [Export] AudioStreamPlayer3D SFX;
    [Export] AudioStream Place;
    [Export] AudioStream Dissapear;
    public Player plr;
    int countdown;
    int ticks;
    Color startColor;
    Vector3 startScale;
    bool sizeDone = false;
    public void UpdateFromTicks(int tick)
    {
        if (tick <= 150)
        {
            float t = Mathf.Clamp((float)tick / 150, 0f, 1f);
            material.AlbedoColor = startColor.Lerp(Colors.Black, t);
        }
        else
        {
            material.AlbedoColor = Colors.Black;

            int scaleTick = tick - 150;
            float t = Mathf.Clamp((float)scaleTick / 2, 0f, 1f);

            Scale = startScale.Lerp(Vector3.One * 0.01f, t);

            if (!sizeDone && scaleTick >= 2)
            {
                sizeDone = true;
                Begone();
            }
        }
    }
    public void Init()
    {
        if (!enabled) return;
        SFX.Stream = Place;
        SFX.Play();
        countdown = (int)(LifeTime*30);
        // Life = new Timer();
        // Life.WaitTime = LifeTime;
        // Tween tween = GetTree().CreateTween();
        material = (StandardMaterial3D)mat.Duplicate();
        startColor = material.AlbedoColor;
        startScale = Scale;
        // tween.TweenProperty(material, "albedo_color", Colors.Black, 5.0f);
        // tween.TweenProperty(this, "scale", Vector3.One * 0.01f, 0.05f);
        mesh.MaterialOverride = material;
        // tween.TweenCallback(Callable.From(Begone));
    }
    public override void _Process(double _delta)
    {
        ticks = boss.i;
        int sidgeonpex = ticks - spawnedontick;
        
        if (ticks < spawnedontick && Visible && enabled)
        {
            Begone();
            return;
        }

        if (sidgeonpex >= 0 && sidgeonpex < countdown)
        {
            if (!Visible || !enabled)
            {
                Visible = true;
                enabled = true;
                SFX.Stream = Place;
                SFX.Play();
            }

            UpdateFromTicks(sidgeonpex);
            return;
        }

        if (sidgeonpex >= countdown && Visible && enabled)
        {
            Begone();
            return;
        }
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
