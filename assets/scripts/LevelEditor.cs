using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Godot;
using LOSUtils;
using static LOSUtils.ObjectInfo;

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
    [Export] private PackedScene PlatformObject;
    [Export] private PackedScene CheckpointObject;
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
    Node ObjectsTree;
    int amountofplatforms = 0;
    //IMPORTANT
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
        if (SELECTED is OBJECT_Checkpoint check)
        {
            var flag = (CheckpointFlags) check.Flags;
            flag.ReplenishAmount = (int)amount;
        }
    }
    public void UpdateSelector(bool UpdateUI = false, Editor_Object Body = null, bool reset = false)
    {
        if (Body != null) SELECTED = Body;
        TS = new Vector3(0.1f,0.1f,0.1f) + SELECTED.Scale;
        TP = SELECTED.Position;
        if (UpdateUI) UpdateUIProperties(false);
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
        if (!SELECTED.Deletable) return;
        SELECTED.QueueFree();
        InspectElements.Visible = false;
        SELECTED = null;
        HideMarkers();
    }
    private void Duplicate()
    {
        if (SELECTED == null) return;
        if (!SELECTED.Deletable) return;
        SFX.PitchScale = (float)GD.RandRange(0.8,0.95);
        SFX.Stream = SeLect;
        SFX.Play();
        Editor_Object dup = (Editor_Object)SELECTED.Duplicate();
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
            if (!key.Echo && key.Pressed && SELECTED != null && !UI.IsMouseOverUI(GetViewport())) {
                if (key.KeyLabel == Key.Backspace || key.KeyLabel == Key.Delete){
                        DeleteSelected();
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
                if (collider.GetParent() is Editor_Object test && resultt.Count == 0)
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
    [Export] private VBoxContainer InspectElements; 
    bool excusethis = false;
    private void UpdateUIProperties(bool newselected)
{
    if (SELECTED == null)
    {
        InspectElements.Visible = false;
        return;
    }

        InspectElements.Visible = true;

        var members = ObjectUtils.GetEditorMembers(SELECTED.Flags);

        foreach(var kid in InspectElements.GetChildren())
        {
            kid.QueueFree();
        }

        foreach (var member in members)
        {
            object value = ObjectUtils.GetMemberValue(member, SELECTED.Flags);

            GD.Print($"{member.Name} = {value}");

            Control control = CreateControlForValue(member, SELECTED.Flags, value);

            InspectElements.AddChild(control);
        }
    }
    private Control CreateControlForValue(MemberInfo member, object obj, object value)
    {
        HBoxContainer row = new HBoxContainer();
        row.CustomMinimumSize = new Vector2(25,25);

        Label label = new Label();
        label.Text = member.Name;
        row.AddChild(label);

        // FLOAT / INT → SpinBox
        if (value is int || value is float || value is double)
        {
            var spin = new SpinBox();
            spin.Value = Convert.ToDouble(value);

            spin.ValueChanged += (newVal) =>
            {
                ObjectUtils.SetMemberValue(member, obj, Convert.ChangeType(newVal, value.GetType()));
            };

            row.AddChild(spin);
            return row;
        }

        // BOOL → CheckBox
        if (value is bool b)
        {
            var check = new CheckBox();
            check.ButtonPressed = b;

            check.Toggled += (pressed) =>
            {
                ObjectUtils.SetMemberValue(member, obj, pressed);
            };

            row.AddChild(check);
            return row;
        }

        // ENUM → OptionButton
        if (value != null && value.GetType().IsEnum)
        {
            var dropdown = new OptionButton();

            var names = Enum.GetNames(value.GetType());
            foreach (var n in names)
                dropdown.AddItem(n);

            dropdown.Selected = (int)value;

            dropdown.ItemSelected += (index) =>
            {
                var enumValue = Enum.Parse(value.GetType(), names[index]);
                ObjectUtils.SetMemberValue(member, obj, enumValue);
            };

            row.AddChild(dropdown);
            return row;
        }

        // fallback → read-only label
        Label fallback = new Label();
        fallback.Text = value?.ToString() ?? "null";
        row.AddChild(fallback);

        return row;
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
        ObjectsTree = LevelGroup.GetNode<Node3D>("Objects");
        ObjectUtils.Reset();
        if (saving)
        {
            currentfile = LevelName;
            CLI = new CommunityLevelInfo();
            CLI.Name = currentfile;
            CLI.StartAmountPlatforms = amountofplatforms;
            GD.Print(LevelName);
            GD.Print(currentfile);
            Godot.Collections.Array<Node> kids = ObjectsTree.GetChildren();
            foreach (Node kid in kids)
            {
                if (kid is Editor_Object obj) {
                    CLI.Objects.Add(ObjectUtils.SerializeObject(obj));
                }
            }
            CLI.Version = LOSGlobals.EditorVersion;
            CommunityLevel.Save(CLI);
        } else
        {
            currentfile = LevelName;
            string ERROR = "[center]";
            SELECTED=null;
            HideMarkers();
            UpdateUIProperties(true);
            Godot.Collections.Array<Node> kids = ObjectsTree.GetChildren();
            foreach (Node _kid in kids)
            {
                _kid.QueueFree();
            }
            CommunityLevelInfo Level = CommunityLevel.Read(LevelName);
            amountofplatforms = Level.StartAmountPlatforms;
            foreach (ObjectInfo.Object obj in Level.Objects)
            {
                ObjectUtils.CreateObject(ObjectsTree,obj.Flags,obj.Scale,obj.Position);
            }
            if (!ObjectUtils.ContainsFinishLine())
            {
                ERROR += "[color=#ffff67]WARNING: The Finish Line was Not Found in the Level File![/color]\n";
                ObjectUtils.CreateObject(ObjectsTree,new CheckpointFlags(){Checkpoint = CHECKPOINT_TYPES.FINISHLINE},CommunityLevelInfo.Vector3.One,CommunityLevelInfo.Vector3.Zero);
            }
            if (LOSGlobals.EditorVersion.Split(".")[1] != Level.Version.Split(".")[1] || LOSGlobals.EditorVersion.Split(".")[0] != Level.Version.Split(".")[0]) ERROR += $"[color=#ff6767]WARNING: Level Version Mismatch, incompatible versions!\nLevel Version: {Level.Version}\nCurrent Version: {LOSGlobals.EditorVersion}[/color]";
            else if (LOSGlobals.EditorVersion != Level.Version) ERROR += $"[color=#ffff67]WARNING: Level Version Mismatch, this might cause some issues!\nLevel Version: {Level.Version}\nCurrent Version: {LOSGlobals.EditorVersion}[/color]";
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
    Editor_Object SELECTED;

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
        Selector.Scale = Selector.Scale.Lerp((TS+OFFSETSCALE)*dis,(float)delta*16f);
        Selector.Position = Selector.Position.Lerp(TP+OFFSETPOSITION,(float)delta*16f);
        Desired.Position = TP;
        Desired.Scale = TS - new Vector3(0.1f,0.1f,0.1f);
        Vector3 temporary = Desired.Scale;
        Vector3 XORIGIN = Desired.Position + new Vector3(1.0f+(temporary.X/2.0f),0,0);
        Vector3 YORIGIN = Desired.Position + new Vector3(0,1.0f+(temporary.Y/2.0f),0);
        Vector3 ZORIGIN = Desired.Position + new Vector3(0,0,1.0f+(temporary.Z/2.0f));
        if (Touching.X == 0 && X.Visible) {
            X.Position = X.Position.Lerp(XORIGIN+OFFSETPOSITION,20f*(float)delta);
            X.GetNode<Area3D>("Area3D").Scale = Vector3.One;
            X.GetNode<Area3D>("Area3D").Rotation = Vector3.Zero;
        }
        if (Touching.Y == 0 && Y.Visible) {
            Y.Position = Y.Position.Lerp(YORIGIN+OFFSETPOSITION,20f*(float)delta);
            Y.GetNode<Area3D>("Area3D").Scale = Vector3.One;
            Y.GetNode<Area3D>("Area3D").Rotation = Vector3.Zero;
        }
        if (Touching.Z == 0 && Z.Visible) {
            Z.Position = Z.Position.Lerp(ZORIGIN+OFFSETPOSITION,20f*(float)delta);
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
            UpdateUIProperties(false);
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
