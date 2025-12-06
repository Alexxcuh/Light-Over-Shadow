using Godot;
using LOSUtils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;

public partial class Freecam : Node3D
{
    [Export] MeshInstance3D player;
    [Export] Camera3D Cam;
    [Export] Node3D yBone;
    [Export] Panel PauseMenu;
    [Export] Control contr;
    [Export] Control NormalMenu;
    public PackedScene mmnu;
    [Export] Timer TickTimer;
    [Export] Label TimeLabel;
    private float GuessTime;
    private float ActualTime;
    [Export] LightPlatform LightPlatform;
    [Export] public string Level = "Tutorial";
    [Export] private Node3D LevelGroup;
    [ExportGroup("UI")]
    [Export] HSlider Timeline;
    [Export] Button Pauseb;
    [Export] OptionButton Options;
    [Export] CompressedTexture2D Pauseic;
    [Export] CompressedTexture2D Playic;

    private bool DemoPaused;
    public Godot.Collections.Array<Vector4> Positions = [];
    public Vector4 Pos;
    public Vector2 MouseVel;
    public float Zoom = 5f;
    public float MaxZoom = 15f;
    public float MinZoom = 1f;
    public float ZoomStep = 0.25f;
    (Godot.Collections.Array<Vector4> Platforms, float[] times, string Level, Godot.Collections.Array<Vector4> Poss) b;
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouse)
        {
            if (Input.IsActionPressed("RC")) {
                Input.MouseMode = Input.MouseModeEnum.Captured;
                MouseVel += mouse.Relative * 0.025f;
            } else
            {
                Input.MouseMode = Input.MouseModeEnum.Visible;
            }
        }
        if (@event is InputEventMouseButton button)
        {
            if (button.ButtonIndex == MouseButton.WheelDown)
            {
                Zoom += ZoomStep*2f;
                Zoom = Mathf.Clamp(Zoom, MinZoom, MaxZoom);
            }
            else if (button.ButtonIndex == MouseButton.WheelUp)
            {
                Zoom -= ZoomStep*2f;
                Zoom = Mathf.Clamp(Zoom, MinZoom, MaxZoom);
            }
        }
    }
    public void LoadMap(string name)
    {
        if (LevelGroup.GetChildCount() > 0)
        {
            foreach (Node3D e in LevelGroup.GetChildren())
            {
                e.QueueFree();
            }
        }
        PackedScene Map = ResourceLoader.Load<PackedScene>($"res://assets/scenes/Levels/{name}.tscn");
        Node3D Scene = Map.Instantiate<Node3D>();
        if (IsInstanceValid(Scene.GetNodeOrNull<Player>("Player"))) Scene.GetNodeOrNull<Player>("Player").enabled = false;
        LevelGroup.AddChild(Scene);
    }
    int[] PlatformTicks;
    public void Selec(int index)
    {
        b = Demo.LoadDemo(Options.GetItemText(index));
        Level = b.Level;
        Init();
        Discord drpc = GetTree().Root.GetNode<Discord>("DiscordRPC");
        float EndTime = b.times[^1];
        drpc.UpdatePresence($"Watching Demo File: {Options.GetItemText(index)}.ADT",$"End Time of {(int)(EndTime / 60 % 60):00}:{(int)(EndTime % 60):00}.{(int)(EndTime * 100 % 100):00}");
    }
    public override void _Ready()
    {
        Pause();
        base._Ready();
        foreach (string item in Demo.GetDemos()) {
            Options.AddItem(item.Split(".ADT")[0]);
        }
        Options.Selected = Options.ItemCount-1;
        Selec(Options.ItemCount-1);
    }
    public void Init()
    {
        EmitSignal(SignalName.Reset);
        LoadMap(Level);
        Positions = b.Poss;
        PlatformTicks = b.Platforms.Select(static v => Mathf.RoundToInt(v.W)).ToArray();
        i = 0;
        GuessTime = 0;
        maxi = Positions.Count;
        Timeline.Value = i;
        Timeline.MaxValue = maxi;
        DemoPaused = true;
        Pauseb.Icon = Playic;
    }
    private bool _updatingTimeline = false;

    public void DemoPause()
    {
        if (i==maxi) {i=0; pc=0;};
        DemoPaused = !DemoPaused;
        Pauseb.Icon = DemoPaused ? Playic : Pauseic;
    }

    public void Changed(float value)
    {
        if (_updatingTimeline) return;

        int target = (int)value;

        if (target != i)
        {
            i = target;
            DemoPaused = true;
            Pauseb.Icon = Playic;
            pc=MathA.Compare(PlatformTicks, i);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        RotationDegrees -= new Vector3(0, MouseVel.X, 0);
        yBone.RotationDegrees -= new Vector3(MouseVel.Y, 0, 0);
        yBone.RotationDegrees = new Vector3(Mathf.Clamp(yBone.RotationDegrees.X, -90, 90), 0, 0);
        MouseVel *= 0.9f;
        Cam.Position = new Vector3(
            0,
            0,
            Mathf.Lerp(Cam.Position.Z,Zoom,ZoomStep/2f)
        );
        if (Input.IsActionJustPressed("Jump")) DemoPause();
        if (i==maxi) {DemoPaused = true; Pauseb.Icon=Playic;}
        _updatingTimeline = true;

        Timeline.Value = i;

        _updatingTimeline = false;

        if (i<maxi) GuessTime += 1f * (float)delta;
        player.Position = new Vector3(

            (float)Mathf.Lerp(player.Position.X,Pos.X,0.15f),
            (float)Mathf.Lerp(player.Position.Y,Pos.Y,0.15f),
            (float)Mathf.Lerp(player.Position.Z,Pos.Z,0.15f)

        );
        base._PhysicsProcess(delta);
        if (Input.IsActionJustPressed("Pause"))
        {
            Pause();
        }
        TimeLabel.Text = $"{(int)(ActualTime / 60 % 60):00}:{(int)(ActualTime % 60):00}.{(int)(ActualTime * 100 % 100):00}";
    }
    public void MainMenu()
    {
        mmnu = ResourceLoader.Load<PackedScene>("res://assets/scenes/menu.tscn");
        GetTree().ChangeSceneToPacked(mmnu);
    }
    int lasti = 0;
    int i = 0;
    int maxi = 0;
    int pc = 0;
    public void Tick()
    {
        TickTimer.Start();
        if (i >= maxi) return;
        if (Pos.W > TickTimer.WaitTime) Pos.W -= (float)TickTimer.WaitTime;
        else
        {
            if (!DemoPaused)i++;
            if (i < maxi) Pos = Positions[i];
        }
        ActualTime = b.times[i];
        if (PlatformTicks.Contains(i))
        {
            pc=MathA.Compare(PlatformTicks, i)-1;
            LightPlatform Platform = (LightPlatform)LightPlatform.Duplicate();
            Platform.enabled = true;
            Platform.Position = new Vector3(b.Platforms[pc].X,b.Platforms[pc].Y,b.Platforms[pc].Z);
            Platform.Visible = true;
            LevelGroup.AddChild(Platform);
            Platform.Init();
            this.Reset += () =>
            {
                if (IsInstanceValid(Platform))
                    Platform.QueueFree();
            };
        }
    }
    public void Controls()
    {
        contr.Visible = true;
        NormalMenu.Visible = false;
    }
    public void Quit()
    {
        GetTree().Quit();
    }
    bool paused = true;
    public void Pause(int fixd = -1, bool end = false)
    {
        if (fixd == -1) paused = !paused;
        else paused = fixd != 0;
        PauseMenu.Visible = paused;
        Engine.TimeScale = paused ? 0 : 1;
        contr.Visible = false;
        NormalMenu.Visible = true;
    }
    [Signal]
    public delegate void ResetEventHandler();
    public void Restart()
    {
        EmitSignal(SignalName.Reset);
        pc = 0;
        i = 0;
        Pos = Vector4.Zero;
        PauseMenu.GetNode<Control>("NormalMenu").GetNode<Label>("Text").Text = "Paused";
        Pause(0);
        GuessTime = 0;
    }
}
