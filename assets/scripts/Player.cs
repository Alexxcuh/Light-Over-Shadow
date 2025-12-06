using Godot;
using LOSUtils;
using System.Collections.Generic;
using System.Linq;
using DiscordRPC;
public partial class Player : CharacterBody3D
{
    public bool enabled = true;
    public float Speed = 2f;
    public const float JumpVelocity = 5.0f;
    public float death_line = -50f;
    public Vector3 SpawnPoint;
    private Vector3 StartPoint;
    private bool levelended = false;
    [Export] private Camera3D camera;
    [Export] private Node3D xBone;
    [Export] private Node3D yBone;
    [Export] private CollisionShape3D Collision;
    [Export] private Timer Coyote;
    [Export] private RayCast3D HeadBonk;
    [Export] private RayCast3D PlaceSomewhere;
    [Export] private MeshInstance3D LightPlatformVis;
    [Export] public int StartPlatforms = 2;
    public int lightstuff = 0;
    [Export] private Label light;
    [Export] private LightPlatform LightPlatform;
    [Export] public Node3D RootScene;
    [Export] private Label TimeLabel;
    public bool finished;
    public float Time;
    [Export] AudioStreamPlayer3D SFX;
    [Export] AudioStream Jump;
    [Export] AudioStream Crouch;
    [Export] Panel PauseMenu;
    [Export] PackedScene NextLevel = null;
    [Export] Godot.Button Next;
    [Export] Godot.Button DemoSave;
    private PackedScene mmnu;
    [Export] private Control contr;
    [Export] private Control NormalMenu;
    [Export] private Timer TickTimer;
    [Export] private LightPlatform Platform;
    public Godot.Collections.Array<Vector4> Positions = [];
    public float[] Times = [];
    public Godot.Collections.Array<Vector4> PlatformTicks = [];
    public bool paused = false;
    public Discord DRPC;
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouse && !paused && enabled)
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
            Vector3 rotdg = yBone.RotationDegrees;
            rotdg -= new Vector3(mouse.Relative.Y * 0.3f, 0, 0);
            xBone.RotationDegrees -= new Vector3(0, mouse.Relative.X * 0.3f, 0);
            rotdg.X = Mathf.Clamp(rotdg.X, -90, 90);
            yBone.RotationDegrees = rotdg;
        }
    }

    public override void _Ready()
    {
        Discord drpc = GetTree().Root.GetNodeOrNull<Discord>("DiscordRPC");

        if (drpc != null && drpc.client != null && enabled) drpc.UpdatePresence($"Playing {GetTree().CurrentScene.Name}");
        SpawnPoint = Position;
        StartPoint = SpawnPoint;
        lightstuff = StartPlatforms;
        if (NextLevel == null) Next.Visible = false;
    }
    public void NextLvl()
    {
        GetTree().ChangeSceneToPacked(NextLevel);
    }
    public void Controls()
    {
        contr.Visible = true;
        NormalMenu.Visible = false;
    }
    bool temp = false;
    bool doorstuck = false;
    public void SaveDemo()
    {
        if (paused && levelended) {
            Demo.SaveDemo(this);
            DemoSave.Visible = false;
        }
    }
    public void Pause(int fixd = -1, bool end = false)
    {
        if (levelended == false && !contr.Visible && enabled)
        {
            if (fixd == -1) paused = !paused;
            else paused = fixd != 0;
            if (end)
            {
                DemoSave.Visible = true;
                LevelData SaveData = SaveFile.Read();
                PauseMenu.GetNode<Control>("NormalMenu").GetNode<RichTextLabel>("Text").Text = $"Level Finished!\n[font_size=25][color=#ff6]{TimeLabel.Text}[/color]";
                if (SaveData.Levels.GetValueOrDefault(RootScene.Name) != null && SaveData != new LevelData{}){
                    float TimeSave = SaveData.Levels[RootScene.Name].Time;
                    PauseMenu.GetNode<Control>("NormalMenu").GetNode<RichTextLabel>("Text").Text += $"\nPB: [color=#6f6]{(int)(TimeSave / 60 % 60):00}:{(int)(TimeSave % 60):00}.{(int)(TimeSave * 100 % 100):00}[/color]";
                }
                Positions.Add(new Vector4(Position.X,Position.Y,Position.Z,(float)(b*TickTimer.WaitTime)));
                ticks++;
                Times = [.. Times, Time];
                b=0;
                if (RootScene.Name.ToString() != "SaveWatch"){
                    if (SaveData.Levels.GetValueOrDefault(RootScene.Name) != null) {
                        if (SaveData.Levels[RootScene.Name].Time > Time) SaveFile.Save(this);
                    } else SaveFile.Save(this);
                };
            }
            PauseMenu.Visible = paused;
            if (paused) Input.MouseMode = Input.MouseModeEnum.Visible;
            Engine.TimeScale = paused ? 0 : 1;
            levelended = end;
        }
        recording = !paused;
        contr.Visible = false;
        NormalMenu.Visible = true;
    }
    [Signal]
    public delegate void ResetEventHandler();
    public void Restart()
    {
        EmitSignal(SignalName.Reset);
        Positions = [];
        PlatformTicks = [];
        levelended = false;
        PauseMenu.GetNode<Control>("NormalMenu").GetNode<RichTextLabel>("Text").Text = "Paused";
        Pause(0);
        Time = 0;
        ticks = 0;
        SpawnPoint = StartPoint;
        Position = SpawnPoint;
        Times = [];
        Velocity = Vector3.Zero;
        lightstuff = StartPlatforms;
        finished = false;

    }
    public void Quit()
    {
        GetTree().Quit();
    }
    public void MainMenu()
    {
        mmnu = ResourceLoader.Load<PackedScene>("res://assets/scenes/menu.tscn");
        GetTree().ChangeSceneToPacked(mmnu);
    }
    int ticks = 0;
    int b = 0;
    bool recording = false;
    public void Tick()
    {
        if (!enabled) return;
        TickTimer.Start();
        Vector4 Pos = Positions.LastOrDefault().Round();
        if ((Position*100f).Round()/10f != new Vector3(Pos.X,Pos.Y,Pos.Z)){
            Positions.Add(new Vector4(Position.X,Position.Y,Position.Z,(float)(b*TickTimer.WaitTime)));
            ticks++;
            Times = [.. Times, Time];
            b=0;
            return;
        }
        b++;
    }
    bool justincase = false;
    public override void _PhysicsProcess(double delta)
    {
        if (!enabled) {
            if (justincase == false)
            {
                xBone.QueueFree();
                Collision.QueueFree();
                TickTimer.QueueFree();
                justincase = true;
            }
            return;
        }
        if (Input.IsActionJustPressed("Pause"))
        {
            Pause();
        }
        if (!paused)
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
            if (!finished) Time += 1f * (float)delta;
            Vector3 velocity = Velocity;
            if (!IsOnFloor())
            {
                velocity += GetGravity() * (float)delta * 3.0f;
                if (velocity.Y == Mathf.Abs(velocity.Y)) temp = true;
                if (!temp)
                {
                    Coyote.Start();
                    temp = true;
                }
            }
            else
            {
                temp = false;
                Coyote.Stop();
            }
            bool FloorCoyote = IsOnFloor() || Coyote.TimeLeft != 0;
            if (Input.IsActionPressed("Jump") && FloorCoyote)
            {
                SFX.Stream = Jump;
                SFX.Play();
                velocity.Y = JumpVelocity * 1.5f;
                if (FloorCoyote) Coyote.Stop();
            }
            if (Input.IsActionJustPressed("Ctrl"))
            {
                if (Collision.Scale == new Vector3(1, 0.5f, 1)) return;
                SFX.Stream = Crouch;
                SFX.Play();
                if (IsOnFloor()) Position -= new Vector3(0, 0.75f, 0);
                Collision.Scale = new Vector3(1, 0.5f, 1);
                Collision.Position = new Vector3(0, 0.25f, 0);
            }
            if (Input.IsActionJustPressed("Restart"))
            {
                Restart();
            }
            else if (Input.IsActionJustPressed("Backspace"))
            {
                Position = SpawnPoint;
                Velocity = Vector3.Zero;
            }
            else if (Input.IsActionJustReleased("Ctrl"))
            {
                if (HeadBonk.GetCollider() != null) { doorstuck = true; return; }
                ;
                Speed = 2f;
                if (IsOnFloor()) Position += new Vector3(0, 0.25f, 0);
                Collision.Scale = Vector3.One;
                Collision.Position = Vector3.Zero;
            }
            if (doorstuck && HeadBonk.GetCollider() == null)
            {
                doorstuck = false;
                Speed = 2f;
                if (IsOnFloor()) Position += new Vector3(0, 0.25f, 0);
                Collision.Scale = Vector3.One;
                Collision.Position = Vector3.Zero;
            }
            if (IsOnFloor() && Input.IsActionPressed("Ctrl")) Speed = 0.8f;
            if (PlaceSomewhere.IsColliding() && PlaceSomewhere.GetCollider() is Area3D floor && Input.IsActionPressed("RC") && lightstuff > 0)
            {
                if (floor.Name == "Floor")
                {
                    LightPlatformVis.Visible = true;
                    LightPlatformVis.Position = new Vector3(1, 0.1f, 1) * PlaceSomewhere.GetCollisionPoint();
                    Engine.TimeScale = 0.6f; //slowmo when placing light platforms :D
                    if (Input.IsActionJustPressed("LC"))
                    {
                        LightPlatform Platform = (LightPlatform)LightPlatform.Duplicate();
                        Platform.enabled = true;
                        Platform.Position = LightPlatformVis.Position;
                        Platform.Visible = true;
                        RootScene.AddChild(Platform);
                        Platform.plr = this;
                        Platform.Init();
                        PlatformTicks.Add(new Vector4(Platform.Position.X,Platform.Position.Y,Platform.Position.Z,ticks));
                        this.Reset += () =>
                        {
                            if (IsInstanceValid(Platform))
                                Platform.QueueFree();
                        };
                        lightstuff--;
                    }
                }
            }
            else
            {
                LightPlatformVis.Visible = false;
                Engine.TimeScale = 1f;
            }
            ;
            Vector2 inputDir = Input.GetVector("A", "D", "W", "S");
            Vector3 forwardDir = xBone.GlobalTransform.Basis.Z;
            forwardDir = forwardDir.Normalized();

            Vector3 rightDir = xBone.GlobalTransform.Basis.X;
            rightDir = rightDir.Normalized();

            Vector3 direction = (forwardDir * inputDir.Y + rightDir * inputDir.X).Normalized();

            velocity.X += direction.X * Speed;
            velocity.Z += direction.Z * Speed;
            if(!IsOnFloor())
            {
                velocity.X *= 1.02f;
                velocity.Z *= 1.02f;
            }
            velocity.X *= 0.75f;
            velocity.Z *= 0.75f;
            Velocity = velocity;

            light.Text = $"Platforms: {lightstuff}";
            TimeLabel.Text = $"{(int)(Time / 60 % 60):00}:{(int)(Time % 60):00}.{(int)(Time * 100 % 100):00}";
            if (Position.Y <= death_line) Restart();
            MoveAndSlide();
        }
	}
}
