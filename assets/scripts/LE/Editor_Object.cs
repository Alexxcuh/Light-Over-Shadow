using Godot;
using LOSUtils;
using static LOSUtils.ObjectInfo;

public abstract partial class Editor_Object : MeshInstance3D
{
    public int ID {get; set;}
    [EditorVisible]
    public abstract ObjectFlags Flags {get;set;}
    [Export] public bool Deletable {get;set;} = true;
    [Export] public bool Duplicateable {get;set;} = true;
}