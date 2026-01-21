using System.Data;
using Godot;
using LOSUtils;

public partial class LevelEditor : Node3D
{
#region BASE ELEMENTS
    // have yet to implement playtesting [Export] MeshInstance3D player;
    [Export] Camera3D Cam;
    [Export] Node3D yBone;
    [Export] Panel PauseMenu;
    [Export] Control contr;
    [Export] Control NormalMenu;
    public PackedScene mmnu;
    [Export] private AudioStreamPlayer SFX;
    [Export] private AudioStreamPlayer UISFX;
    [Export] private AudioStream ReSize;
    [Export] private AudioStream SeLect;
    [Export] private AudioStream SeLect_2;
    [Export] private AudioStream SeLect_3;
    [Export] Label TimeLabel;
    private float GuessTime;
    private float ActualTime;
    [Export] public string Level = "Tutorial";
    [Export] private Node3D LevelGroup;
    private bool DemoPaused;
    public Godot.Collections.Array<Vector3> Positions = [];
    public Vector3 Pos;
    public Vector2 MouseVel;
    public Discord DRPC;
    private SaveData Save;
    public float SENSITIVITY = 3f;
    [Export] private MeshInstance3D Selector;
    [Export] private MeshInstance3D Desired;
    [Export] private ArrayMesh UIArrow;
    [Export] private ArrayMesh UISphere;
    [Export] private PackedScene PlatformScene;
    [Export] private PackedScene CheckpointScene;
    [Export] private VBoxContainer Delete;
    //checkpoint UI stuff
    [Export] private VBoxContainer CheckpointUI;
    [Export] private CheckBox ReplenisherUI;
    [Export] private SpinBox ReplenishAmount;
    //player UI stuff
    [Export] private VBoxContainer PlayerUI;
    [Export] private SpinBox PlatformsAmount;
    Vector3 TS;
    Vector3 TP;
    bool holding = false;
    Vector2 MousePos;
    Vector3 HitPos;
    Node collider;
    Node colliderr;
    int amountofplatforms = 0;
    //IMPORTANT
    [Export] private string version = "1.0.1";
    Godot.Collections.Dictionary result;
    Godot.Collections.Dictionary resultt;
    #endregion
    #region INPUT HANDLING
    private void ReplenishVal(float amount)
    {
        if (excusethis) return;
        amountofplatforms = (int)amount;
    }
    private void ChangePlatforms(float amount)
    {
        if (excusethis) return;
        if (SELECTED is Finish point)
        {
            point.ReplenishLight = (int)amount;
        }
    }
    private void ReplenisherToggled()
    {
        if (excusethis) return;
        bool toggle = !ReplenisherUI.ToggleMode;
        if (SELECTED is Finish b) {
            b.replenisher = toggle;
            b.UpdateMaterials();
        }
        GD.Print(toggle);
        UpdateUIProperties();
    }
    public void UpdateSelector(bool UpdateUI = false, Node3D Body = null, bool reset = false)
    {
        if (Body != null) SELECTED = Body;
        TS = new Vector3(0.1f,0.1f,0.1f) + SELECTED.Scale;
        TP = SELECTED.Position;
        if (UpdateUI) UpdateUIProperties();
        if (reset)
        {
            Selector.Position = TP;
            Selector.Scale = SELECTED.Scale;
            if (X.Visible && Y.Visible && Z.Visible) {
                Vector3 XORIGIN = Selector.Position + new Vector3(Selector.Scale.X/2.0f,0,0);
                Vector3 YORIGIN = Selector.Position + new Vector3(0,Selector.Scale.Y/2.0f,0);
                Vector3 ZORIGIN = Selector.Position + new Vector3(0,0,Selector.Scale.Z/2.0f);
                X.Position = XORIGIN;
                Y.Position = YORIGIN;
                Z.Position = ZORIGIN;
            }
        }
    }
    private void DeleteSelected()
    {
        if (SELECTED == null) return;
        SELECTED.QueueFree();
        InspectElements.Visible = false;
        SELECTED = null;
        HideMarkers();
    }
    private void Duplicate()
    {
        SFX.PitchScale = (float)GD.RandRange(0.8,0.95);
        SFX.Stream = SeLect;
        SFX.Play();
        if (SELECTED == null) return;
        Node3D dup = (Node3D)SELECTED.Duplicate();
        SELECTED.GetParent().AddChild(dup);
        dup.Position += new Vector3(0,0.5f,0);
        UpdateSelector(true,dup);
    }
    bool CtrlPressed;
    public override void _UnhandledInput(InputEvent @event)
    {
        if (paused) return;
        if (@event is InputEventKey key)
        {
            CtrlPressed = key.CtrlPressed;
            if (!key.Pressed) CtrlPressed = false;
            if (SELECTED == null) return;
            if (SELECTED.Name != "FINISH" && !key.Echo && key.Pressed && SELECTED != null && !UI.IsMouseOverUI(GetViewport())) {
                if (key.KeyLabel == Key.Backspace || key.KeyLabel == Key.Delete){
                    SELECTED.QueueFree();
                    InspectElements.Visible = false;
                    SELECTED = null;
                    HideMarkers();
                }
                if (key.KeyLabel == Key.D && key.CtrlPressed) {
                    Duplicate();
                }
            }
        }
        if (@event is InputEventMouseMotion mouse)
        {
            MousePos = mouse.Position;
            if (Input.IsActionPressed("RC")) {
                Input.MouseMode = Input.MouseModeEnum.Captured;
                MouseVel = mouse.Relative * (SENSITIVITY/10.0f);
            } else
            {
                Input.MouseMode = Input.MouseModeEnum.Visible;
            }
        }
        if (@event is InputEventMouseButton eventMouseButton
            && eventMouseButton.ButtonIndex == MouseButton.Left)
        {
            if (!eventMouseButton.Pressed)
            {
                holding = false;
                return;
            } else holding = true;
            var query = PhysicsRayQueryParameters3D.Create(from, to);
            query.CollideWithAreas = true;
            query.CollideWithBodies = true;

            result = spaceState.IntersectRay(query);
            if (result.Count > 0 && resultt.Count < 1) {
                collider = (Node)result["collider"];
                HitPos = (Vector3)result["position"];
            }
            if (result.Count > 0)
            {
                if (collider is Node3D test && resultt.Count == 0)
                {
                    if (test != SELECTED) {
                        SFX.PitchScale = (float)GD.RandRange(0.8,0.95);
                        SFX.Stream = SeLect;
                        SFX.Play();
                    }
                    UpdateSelector(true,test);
                    InspectElements.Visible = true;
                }
                if (Selector.Visible == false) {
                    Selector.Position = TP;
                    Selector.Scale = SELECTED.Scale;
                    ShowMarkers();
                }
            }
            else
            {
                SELECTED = null;
                InspectElements.Visible = false;
                HideMarkers();
            }
        }

    }
    // public void LoadMap(string name)
    // {
    //     if (LevelGroup.GetChildCount() > 0)
    //     {
    //         foreach (Node3D e in LevelGroup.GetChildren())
    //         {
    //             e.QueueFree();
    //         }
    //     }
    //     PackedScene Map = ResourceLoader.Load<PackedScene>($"res://assets/scenes/Levels/{name}.tscn");
    //     Node3D Scene = Map.Instantiate<Node3D>();
    //     if (IsInstanceValid(Scene.GetNodeOrNull<Player>("Player"))) Scene.GetNodeOrNull<Player>("Player").enabled = false;
    //     LevelGroup.AddChild(Scene);
    // }
    private int selected = -1;
    // 0 - Scale
    // 1 - Position
    public void BUTTONPRESS()
    {
        SFX.PitchScale = (float)GD.RandRange(0.8,0.95);
        UISFX.Stream = GD.Randf() <= 0.5f ? SeLect_3 : SeLect_2;
        UISFX.Stream = GD.Randf() <= 0.5f ? SeLect : UISFX.Stream;
        UISFX.Play();
    }
    public void Select(int sel = -1)
    {
        BUTTONPRESS();
        if (Selector.Visible && selected != sel) {
            Vector3 XORIGIN = Selector.Position + new Vector3(Selector.Scale.X/2.0f,0,0);
            Vector3 YORIGIN = Selector.Position + new Vector3(0,Selector.Scale.Y/2.0f,0);
            Vector3 ZORIGIN = Selector.Position + new Vector3(0,0,Selector.Scale.Z/2.0f);
            X.Position = XORIGIN;
            Y.Position = YORIGIN;
            Z.Position = ZORIGIN;
        }
        selected = sel;
        ArrayMesh modl = UISphere;
        if (selected == 1) modl = UIArrow;
        X.GetNode<MeshInstance3D>("MeshInstance3D").Mesh = modl;
        Y.GetNode<MeshInstance3D>("MeshInstance3D").Mesh = modl;
        Z.GetNode<MeshInstance3D>("MeshInstance3D").Mesh = modl;        
        ShowMarkers();
        if (sel == -1)
            HideMarkers(true);
    }
#endregion
    // HANDLING UI ELEMENTS
    #region UI HANDLING 
    [Export] private HBoxContainer PosBox;
    [Export] private HBoxContainer SclBox;
    [Export] private ScrollContainer InspectElements;
    [Export] private SpinBox PosXInput;
    [Export] private SpinBox PosYInput;
    [Export] private SpinBox PosZInput;
    [Export] private SpinBox SclXInput;
    [Export] private SpinBox SclYInput;
    [Export] private SpinBox SclZInput;   
    bool excusethis = false;
    private void UpdateUIProperties()
    {
        if (SELECTED == null)
        {
            InspectElements.Visible = false;
            return;
        }
        if (SELECTED != null && InspectElements.Visible == false ) InspectElements.Visible = true;
        if (SELECTED.Name == "FINISH") Delete.Visible = false;
        else Delete.Visible = true;
        if (SELECTED is Finish point && SELECTED.Name != "FINISH")
        {
            excusethis = true;
            PlayerUI.Visible = false;
            CheckpointUI.Visible = true;
            ReplenisherUI.ToggleMode = point.replenisher;
            ReplenishAmount.Value = point.ReplenishLight;
            excusethis = false;
        } else
        {
            PlayerUI.Visible = false;
            CheckpointUI.Visible = false;
        }
        PosBox.Visible = true;
        SclBox.Visible = true;
        if (SELECTED.Name == "PLAYER") {
            Delete.Visible = false;
            PlayerUI.Visible = true;
            excusethis = true;
            PosBox.Visible = false;
            SclBox.Visible = false;
            PlatformsAmount.Value = amountofplatforms;
            excusethis = false;
        }
        Vector3 newpos = SELECTED.Position + OFFSETPOSITION;
        Vector3 newscl = SELECTED.Scale + OFFSETSCALE;
        excusethis = true;
        PosXInput.Value = newpos.X;
        PosYInput.Value = newpos.Y;
        PosZInput.Value = newpos.Z;
        SclXInput.Value = newscl.X;
        SclYInput.Value = newscl.Y;
        SclZInput.Value = newscl.Z;
        excusethis = false;
    }
    public void PosX(float value) {
        if (excusethis) return;
        SetPosTo(new Vector3(value,SELECTED.Position.Y,SELECTED.Position.Z));
        return;
    }
    public void PosY(float value) {
        if (excusethis) return;
        SetPosTo(new Vector3(SELECTED.Position.X,value,SELECTED.Position.Z));
        return;
    }
    public void PosZ(float value) {
        if (excusethis) return;
        SetPosTo(new Vector3(SELECTED.Position.X,SELECTED.Position.Y,value));
        return;
    }
    public void SclX(float value) {
        if (excusethis) return;
        SetSclTo(new Vector3(value,SELECTED.Scale.Y,SELECTED.Scale.Z));
        return;
    }
    public void SclY(float value) {
        if (excusethis) return;
        SetSclTo(new Vector3(SELECTED.Scale.X,value,SELECTED.Scale.Z));
        return;
    }
    public void SclZ(float value) {
        if (excusethis) return;
        SetSclTo(new Vector3(SELECTED.Scale.X,SELECTED.Scale.Y,value));
        return;
    }   
#endregion
#region SAVE & LOAD LEVEL
    [Export] private FileDialog pop;
    CommunityLevelInfo CLI;
    bool saving = false;
    [Export] private ErrorPOPUP error;
    private string currentfile;
    public void FileSelect(string path)
    {
        BUTTONPRESS();
        string LevelName = path.Split("/")[^1].Split(".")[0];
        if (saving)
        {
            currentfile = LevelName;
            CLI = new CommunityLevelInfo();
            CLI.Name = currentfile;
            CLI.StartAmountPlatforms = amountofplatforms;
            GD.Print(LevelName);
            GD.Print(currentfile);
            Godot.Collections.Array<Node> kids = LevelGroup.GetNode<Node3D>("Platforms").GetChildren();
            foreach (Node kid in kids)
            {
                if (kid is StaticBody3D body)
                {
                    CommunityLevelInfo.Platform pltfm = new()
                    {
                        Position = new CommunityLevelInfo.Vector3(body.Position.X,body.Position.Y,body.Position.Z),
                        Scale = new CommunityLevelInfo.Vector3(body.Scale.X,body.Scale.Y,body.Scale.Z)
                    };
                    CLI.Platforms.Add(pltfm);
                }
            }
            kids = LevelGroup.GetNode<Node3D>("Checkpoints").GetChildren();
            foreach (Node kid in kids)
            {
                if (kid is Finish checkpoint)
                {
                    CommunityLevelInfo.Checkpoint chkpoint = new()
                    {
                        Position = new CommunityLevelInfo.Vector3(checkpoint.Position.X,checkpoint.Position.Y,checkpoint.Position.Z),
                        Scale = new CommunityLevelInfo.Vector3(checkpoint.Scale.X,checkpoint.Scale.Y,checkpoint.Scale.Z),
                        ReplenishAmount = checkpoint.ReplenishLight,
                        Replenisher = checkpoint.replenisher,
                        Finish = checkpoint.FinishLine
                    };
                    CLI.Checkpoints.Add(chkpoint);
                }
            }
            CLI.Version = version;
            CommunityLevel.Save(CLI);
        } else
        {
            currentfile = LevelName;
            string ERROR = "[center]";
            SELECTED=null;
            HideMarkers();
            UpdateUIProperties();
            // PLATFORMS
            Godot.Collections.Array<Node> kids = LevelGroup.GetNode<Node3D>("Platforms").GetChildren();
            foreach (Node _kid in kids)
            {
                _kid.QueueFree();
            }
            CommunityLevelInfo Level = CommunityLevel.Read(LevelName);
            amountofplatforms = Level.StartAmountPlatforms;
            foreach (CommunityLevelInfo.Platform platform in Level.Platforms)
            {
                StaticBody3D pltfmr = (StaticBody3D)PlatformScene.Instantiate();
                LevelGroup.GetNode<Node3D>("Platforms").AddChild(pltfmr);
                pltfmr.Scale = new Vector3(platform.Scale.X,platform.Scale.Y,platform.Scale.Z);
                pltfmr.Position = new Vector3(platform.Position.X,platform.Position.Y,platform.Position.Z);
            }
            // CHECKPOINTS
            kids = LevelGroup.GetNode<Node3D>("Checkpoints").GetChildren();
            foreach (Node _kid in kids)
            {
                _kid.QueueFree();
            }
            foreach (CommunityLevelInfo.Checkpoint Checkpoint in Level.Checkpoints)
            {
                Finish chk = (Finish)CheckpointScene.Instantiate();
                chk.replenisher = Checkpoint.Replenisher;
                chk.ReplenishLight = Checkpoint.ReplenishAmount;
                chk.FinishLine = Checkpoint.Finish;
                chk.ineditor = true;
                chk.Scale = new Vector3(Checkpoint.Scale.X,Checkpoint.Scale.Y,Checkpoint.Scale.Z);
                chk.Position = new Vector3(Checkpoint.Position.X,Checkpoint.Position.Y,Checkpoint.Position.Z);
                LevelGroup.GetNode<Node3D>("Checkpoints").AddChild(chk);
                chk.UpdateMaterials();
            }
            if (Level.Checkpoints.Count == 0)
            {
                ERROR += "[color=#ffff67]WARNING: The Finish Line was Not Found in the Level File![/color]\n";
                Finish chk = (Finish)CheckpointScene.Instantiate();
                chk.FinishLine = true;
                chk.ineditor = true;
                chk.Scale = new Vector3(2.5f,0.5f,2.5f);
                chk.Position = new Vector3(-4.0f,0.1f,0);
                LevelGroup.GetNode<Node3D>("Checkpoints").AddChild(chk);
                chk.UpdateMaterials();
            }
            if (version != Level.Version) ERROR += $"[color=#ffff67]WARNING: Level Version Mismatch, this might cause some issues!\nLevel Version: {Level.Version}\nCurrent Version: {version}[/color]";
            if (ERROR != "[center]") error.Popup("Oh Uh!",ERROR);
        }
        if (DRPC != null && DRPC.client != null) DRPC.UpdatePresence($"Editing {currentfile}.AMT");
    }
    public void EndDialog()
    {
        Pause(pop.Visible ? 1 : 0);
    }
    public void SaveLevel()
    {   
        BUTTONPRESS();
        Pause(1);
        pop.Access = FileDialog.AccessEnum.Userdata;
        pop.FileMode = FileDialog.FileModeEnum.SaveFile;
        pop.CurrentDir = "user://Level/";
        pop.PopupCentered();
        saving = true;
        return;
    }
    public void LoadLevel()
    {
        BUTTONPRESS();
        Pause(1);
        pop.Access = FileDialog.AccessEnum.Userdata;
        pop.FileMode = FileDialog.FileModeEnum.OpenFile;
        pop.CurrentDir = "user://Level/";
        pop.PopupCentered();
        saving = false;
        return;
    }
#endregion
#region CAMERA MOVEMENT & MISC
    // END
    public override void _Ready()
    {
        DRPC = GetTree().Root.GetNodeOrNull<Discord>("DiscordRPC");
        if (DRPC != null && DRPC.client != null) DRPC.UpdatePresence($"Editing UNSAVED.AMT");
        PosXInput.Connect(Range.SignalName.ValueChanged,Callable.From<float>(PosX));
        PosYInput.Connect(Range.SignalName.ValueChanged,Callable.From<float>(PosY));
        PosZInput.Connect(Range.SignalName.ValueChanged,Callable.From<float>(PosZ));
        SclXInput.Connect(Range.SignalName.ValueChanged,Callable.From<float>(SclX));
        SclYInput.Connect(Range.SignalName.ValueChanged,Callable.From<float>(SclY));
        SclZInput.Connect(Range.SignalName.ValueChanged,Callable.From<float>(SclZ));
        Pause(0);
        pop.FileSelected += FileSelect;
        Save = SaveFile.Read();
        SENSITIVITY = (float)Save.Settings.MouseSens;
    }


    public void Init()
    {
        EmitSignal(SignalName.Reset);
        i = 0;
    }
    float totaltime = 0f;
    float countdown = 0.033f;
    bool start = true;
    public float Speed = 2f;
    Vector3 velocity;
    [Export] Node3D X;
    [Export] Node3D Y;
    [Export] Node3D Z;
    Node3D SELECTED;

    public override void _PhysicsProcess(double delta)
    {
        Vector2 inputDir = Input.GetVector("A", "D", "W", "S");
        if (UI.IsMouseOverUI(GetViewport()) || CtrlPressed)
        {
            inputDir = Vector2.Zero;
        }
        Vector3 forwardDir = yBone.GlobalTransform.Basis.Z;
        forwardDir = forwardDir.Normalized();

        Vector3 rightDir = yBone.GlobalTransform.Basis.X;
        rightDir = rightDir.Normalized();

        Vector3 direction = (forwardDir * inputDir.Y + rightDir * inputDir.X).Normalized();

        velocity += direction * Speed;
        velocity *= 0.75f;
        Position += velocity*(float)delta;
    }
    Vector3 Touching = Vector3.Zero;
    Vector3 OFFSETSCALE = Vector3.Zero;
    Vector3 OFFSETPOSITION = Vector3.Zero;
    public void HideMarkers(bool disableselected = false)
    {
        X.Position = Vector3.One*9999f;
        Y.Position = X.Position;
        Z.Position = X.Position;
        X.Visible = false;
        Y.Visible = false;
        Z.Visible = false;
        if (!disableselected) Selector.Visible = false;
    }
    public void ShowMarkers()
    {
        if (SELECTED == null) return;
        Selector.Visible = true;
        if (selected == -1) return;
        Vector3 XORIGIN = Selector.Position + new Vector3(Selector.Scale.X/2.0f,0,0);
        Vector3 YORIGIN = Selector.Position + new Vector3(0,Selector.Scale.Y/2.0f,0);
        Vector3 ZORIGIN = Selector.Position + new Vector3(0,0,Selector.Scale.Z/2.0f);
        X.Position = XORIGIN;
        Y.Position = YORIGIN;
        Z.Position = ZORIGIN;
        X.Visible = true;
        Y.Visible = true;
        Z.Visible = true;
    }
    bool red;
    Vector3 Starting;
    float startingpos;
    Vector3 from;
    Vector3 to;
    PhysicsDirectSpaceState3D spaceState;
#endregion
#region UI POSITION
    private void SetPosTo(Vector3 Poss)
    {
        SELECTED.Position = Poss;
        TP = SELECTED.Position;
        Selector.Position = TP+OFFSETPOSITION;
    }
    private void SetSclTo(Vector3 Scll)
    {
        Vector3 OLDSCALE = SELECTED.Scale;
        SELECTED.Scale = Scll;
        UpdateSelector(false);
        if (Scll-OLDSCALE > Vector3.Zero) Selector.Scale = TS+OFFSETSCALE; 
    }
    private void HandleX(Vector3 XORIGIN)
    {
        X.GetNode<Area3D>("Area3D").Scale = new Vector3(155,155,1);
        X.GetNode<Area3D>("Area3D").LookAt(Cam.GlobalPosition);
        X.GetNode<Area3D>("Area3D").Rotation = new Vector3(X.GetNode<Area3D>("Area3D").Rotation.X,X.GetNode<Area3D>("Area3D").Rotation.Y,0);
        if (Touching != new Vector3(1,0,0)) {
            startingpos = HitPos.X - X.Position.X;
            Touching = new Vector3(1,0,0);
        }
        float FX = HitPos.X - startingpos;
        X.Position = new Vector3(
            FX,
            Desired.Position.Y,
            Desired.Position.Z
        );
        float SX = FX - XORIGIN.X;
        var tempX = Mathf.Round(SX*2.0f);
        if (selected == 0) {
            if (Desired.Scale.X + tempX > 0) OFFSETSCALE.X = tempX;
        } else if (selected == 1) {
            OFFSETPOSITION.X = tempX/2.0f;
        }
    }
    private void HandleY(Vector3 YORIGIN)
    {
Y.GetNode<Area3D>("Area3D").Scale = new Vector3(155,155,1);
        Y.GetNode<Area3D>("Area3D").LookAt(Cam.GlobalPosition);
        Y.GetNode<Area3D>("Area3D").Rotation = new Vector3(Y.GetNode<Area3D>("Area3D").Rotation.X,Y.GetNode<Area3D>("Area3D").Rotation.Y,0);
        if (Touching != new Vector3(0,1,0)) {
            startingpos = HitPos.Y - Y.Position.Y;
            Touching = new Vector3(0,1,0);
        }
        float FY = HitPos.Y - startingpos;
        Y.Position = new Vector3(
            Desired.Position.X,
            FY,
            Desired.Position.Z
        );
        float SY = FY - YORIGIN.Y;
        var tempY = Mathf.Round(SY*2.0f);
        if (selected == 0) {
            if (Desired.Scale.Y + tempY > 0) OFFSETSCALE.Y = tempY;
        } else if (selected == 1) {
            OFFSETPOSITION.Y = tempY/2.0f;
        }
    }
    private void HandleZ(Vector3 ZORIGIN)
    {
Z.GetNode<Area3D>("Area3D").Scale = new Vector3(155,155,1);
        Z.GetNode<Area3D>("Area3D").LookAt(Cam.GlobalPosition);
        Z.GetNode<Area3D>("Area3D").Rotation = new Vector3(Z.GetNode<Area3D>("Area3D").Rotation.X,Z.GetNode<Area3D>("Area3D").Rotation.Y,0);
        if (Touching != new Vector3(0,0,1)) {
            startingpos = HitPos.Z - Z.Position.Z;
            Touching = new Vector3(0,0,1);
        }
        float FZ = HitPos.Z - startingpos;
        Z.Position = new Vector3(
            Desired.Position.X,
            Desired.Position.Y,
            FZ
        );
        float SZ = FZ - ZORIGIN.Z;
        var tempZ = Mathf.Round(SZ*2.0f);
        if (selected == 0) {
            if (Desired.Scale.Z + tempZ > 0) OFFSETSCALE.Z = tempZ;
        } else if (selected == 1) {
            OFFSETPOSITION.Z = tempZ/2.0f;
        }
    }
    private void HandleNotMine(Vector3 XORIGIN, Vector3 YORIGIN, Vector3 ZORIGIN)
    {
        if (Touching.X!=0)
        {
            HandleX(XORIGIN);
            return;
        }
        if (Touching.Y!=0)
        {
            HandleY(YORIGIN);
            return;
        }
        if (Touching.Z!=0)
        {
            HandleZ(ZORIGIN);
            return;
        }
        return;
    }
#endregion
#region MARKERS & MOVEMENT
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Pause"))
        {
            Pause();
        }
        if (paused) return;
        from = Cam.ProjectRayOrigin(MousePos);
        to = from + Cam.ProjectRayNormal(MousePos) * 1000f;

        spaceState = GetWorld3D().DirectSpaceState;
        var queryy = PhysicsRayQueryParameters3D.Create(from, to);
        queryy.CollideWithAreas = true;
        queryy.CollideWithBodies = false;
        queryy.CollisionMask = 32768;

        resultt = spaceState.IntersectRay(queryy);
        float dis = 1.0f;
        if (Touching == Vector3.Zero) dis = Mathf.Clamp(1-Selector.Position.DistanceTo(TP) * 0.05f,0.3f,1);
        Selector.Scale = Selector.Scale.Lerp((TS+OFFSETSCALE)*dis,0.4f);
        Selector.Position = Selector.Position.Lerp(TP+OFFSETPOSITION,0.5f);
        Desired.Position = TP;
        Desired.Scale = TS - new Vector3(0.1f,0.1f,0.1f);
        Vector3 temporary = Desired.Scale;
        Vector3 XORIGIN = Desired.Position + new Vector3(1.0f+(temporary.X/2.0f),0,0);
        Vector3 YORIGIN = Desired.Position + new Vector3(0,1.0f+(temporary.Y/2.0f),0);
        Vector3 ZORIGIN = Desired.Position + new Vector3(0,0,1.0f+(temporary.Z/2.0f));
        if (Touching.X == 0 && X.Visible) {
            X.Position = X.Position.Lerp(XORIGIN+OFFSETPOSITION,0.5f);
            X.GetNode<Area3D>("Area3D").Scale = Vector3.One;
            X.GetNode<Area3D>("Area3D").Rotation = Vector3.Zero;
        }
        if (Touching.Y == 0 && Y.Visible) {
            Y.Position = Y.Position.Lerp(YORIGIN+OFFSETPOSITION,0.5f);
            Y.GetNode<Area3D>("Area3D").Scale = Vector3.One;
            Y.GetNode<Area3D>("Area3D").Rotation = Vector3.Zero;
        }
        if (Touching.Z == 0 && Z.Visible) {
            Z.Position = Z.Position.Lerp(ZORIGIN+OFFSETPOSITION,0.5f);
            Z.GetNode<Area3D>("Area3D").Scale = Vector3.One;
            Z.GetNode<Area3D>("Area3D").Rotation = Vector3.Zero;
        }
        if (resultt.Count > 0 && (X.Position.DistanceTo(XORIGIN) < 0.001 || Touching != Vector3.Zero)) {
            colliderr = (Node)resultt["collider"];
            HitPos = (Vector3)resultt["position"];
            if (colliderr is Area3D marker && holding)
            {
                Vector3 OLDOFFSET = Vector3.Zero;
                if (selected == 0) 
                    OLDOFFSET = OFFSETSCALE;
                else if (selected == 1)
                    OLDOFFSET = OFFSETPOSITION;
                switch (marker.GetParent().Name)
                {
                    case "X":
                        if (Touching != new Vector3(1,0,0) && Touching != Vector3.Zero) {
                            HandleNotMine(XORIGIN,YORIGIN,ZORIGIN);
                            break;
                        }
                        HandleX(XORIGIN);
                        break;
                    case "Y":
                        if (Touching != new Vector3(0,1,0) && Touching != Vector3.Zero) {
                            HandleNotMine(XORIGIN,YORIGIN,ZORIGIN);
                            break;
                        }
                        HandleY(YORIGIN);
                        break;
                    case "Z":
                        if (Touching != new Vector3(0,0,1) && Touching != Vector3.Zero) {
                            HandleNotMine(XORIGIN,YORIGIN,ZORIGIN);
                            break;
                        }
                        HandleZ(ZORIGIN);
                        break;
                    default:
                        Touching = Vector3.Zero;
                        break;
                }
                if (SELECTED.Name == "PLAYER"){
                    OFFSETPOSITION = Vector3.Zero;
                    OFFSETSCALE = Vector3.Zero;
                }
                if (selected == 0) {
                    if (OFFSETSCALE != OLDOFFSET){
                        if (SFX.Stream != ReSize) SFX.Stream = ReSize;
                        SFX.PitchScale = 0.8f+((OFFSETSCALE.X+OFFSETSCALE.Y+OFFSETSCALE.Z)*0.005f);
                        SFX.Play();
                    }
                    if (OLDOFFSET < OFFSETSCALE)
                    {
                        Selector.Scale = TS+OFFSETSCALE;
                    }
                } else if (selected == 1) {
                    Selector.Position = TP+OFFSETPOSITION;
                    if (OFFSETPOSITION != OLDOFFSET){
                        if (SFX.Stream != ReSize) SFX.Stream = ReSize;
                        SFX.PitchScale = 0.8f+((OFFSETPOSITION.X+OFFSETPOSITION.Y+OFFSETPOSITION.Z)*0.0025f);
                        SFX.Play();
                    }
                }
            } else
            {
                red = false;
            }
            UpdateUIProperties();
        }
        if (!holding && Touching != Vector3.Zero){
            Touching = Vector3.Zero;
            UpdateSelector(true);
            OFFSETSCALE = Vector3.Zero;
            OFFSETPOSITION = Vector3.Zero;
        }
        else
        {
            Desired.Position += OFFSETPOSITION;
            Desired.Scale += OFFSETSCALE;
            if (resultt.Count > 0 && SELECTED != null) {
                SELECTED.Scale = Desired.Scale;
                SELECTED.Position = Desired.Position;
            }
        }
        if (start) {
            totaltime += (float)delta;
            if (totaltime >= countdown)
            {
                totaltime = 0;
                start = false;
                Tick();
            }
        }
        RotationDegrees -= new Vector3(0, MouseVel.X, 0);
        yBone.RotationDegrees -= new Vector3(MouseVel.Y, 0, 0);
        yBone.RotationDegrees = new Vector3(Mathf.Clamp(yBone.RotationDegrees.X, -90, 90), 0, 0);
        MouseVel = Vector2.Zero;
        TimeLabel.Text = $"{(int)(ActualTime / 60 % 60):00}:{(int)(ActualTime % 60):00}.{(int)(ActualTime * 100 % 100):00}";
    }
#endregion
#region UI MENUS
    public void MainMenu()
    {
        mmnu = ResourceLoader.Load<PackedScene>("res://assets/scenes/menu.tscn");
        GetTree().ChangeSceneToPacked(mmnu);
    }
    public int i = 0;
    public void Tick()
    {
        start = true;
        i++;
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
        i = 0;
        Pos = Vector3.Zero;
        PauseMenu.GetNode<Control>("NormalMenu").GetNode<Label>("Text").Text = "Paused";
        Pause(0);
        GuessTime = 0;
    }
}
#endregion
