using Godot;
using System;

public partial class ItemSpawner : VBoxContainer
{
    [Export] private LevelEditor LevelEditor;
    [Export] private Node3D LevelGroup;
    [Export] private PackedScene AssociatedItem;
    [Export] private int type = 0;
    //0-platform
    //1-checkpoint
    private void Spawn()
    {
        LevelEditor.BUTTONPRESS();
        Node3D Item = (Node3D)AssociatedItem.Instantiate();
        if (type == 1 && Item is Finish point)
        {
            point.ineditor = true;
            point.FinishLine = false;
        }
        Node3D parent = type == 0 ? LevelGroup.GetNode<Node3D>("Platforms"):LevelGroup.GetNode<Node3D>("Checkpoints");
        parent.AddChild(Item);
        LevelEditor.UpdateSelector(true,Item,true);
        LevelEditor.ShowMarkers();
    }
}
