using Godot;
using LOSUtils;
using System;
using static LOSUtils.ObjectInfo;

public partial class ItemSpawner : VBoxContainer
{
    [Export] private LevelEditor LevelEditor;
    [Export] private Node3D LevelGroup;
    [Export] private OBJECT_TYPES type = OBJECT_TYPES.PLATFORM;
    //0-platform
    //1-checkpoint
    private void Spawn()
    {
        LevelEditor.BUTTONPRESS();
        Node3D parent = LevelGroup.GetNode<Node3D>("Objects");
        ObjectFlags flags;
        switch (type)
        {
            case OBJECT_TYPES.PLATFORM:
                flags = new PlatformFlags();
                break;
            case OBJECT_TYPES.CHECKPOINT:
                flags = new CheckpointFlags();
                break;
            default:
                return;
        }
        Editor_Object Item = ObjectUtils.CreateObject(parent,flags,CommunityLevelInfo.Vector3.One,CommunityLevelInfo.Vector3.Zero);
        LevelEditor.UpdateSelector(true,Item,true);
        LevelEditor.ShowMarkers();
    }
}
