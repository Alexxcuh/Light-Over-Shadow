using System;
using Godot;
using LOSUtils;
using static LOSUtils.ObjectInfo;

public partial class OBJECT_Checkpoint : Editor_Object
{
    private CheckpointFlags _flags = new CheckpointFlags();
    [EditorVisible]
    public override ObjectFlags Flags
    {
        get => _flags;
        set => SetFlag(value);
    }
    private void SetFlag(ObjectFlags flag)
    {
        _flags.Clear();
        _flags = (CheckpointFlags)flag;
        _flags.MaterialOutdated += UpdateMaterialsqwe;
        UpdateMaterials();
    }

    private void UpdateMaterialsqwe(object sender, EventArgs e)
    {
        UpdateMaterials();
    }

    [Export] private StandardMaterial3D CheckpointMat = null;
    [Export] private StandardMaterial3D ReplenisherMat = null;
    [Export] private StandardMaterial3D FinishMat = null;
    [Export] private CompressedTexture2D CheckpointImg;
    [Export] private CompressedTexture2D ReplenisherImg;
    [Export] private CompressedTexture2D FinishImg;
    [Export] private StandardMaterial3D bilboard;
    [Export] private CHECKPOINT_TYPES OverrideInitialType = CHECKPOINT_TYPES.CHECKPOINT;
    StandardMaterial3D mat;
    public override void _Ready()
    {
        mat = (StandardMaterial3D)bilboard.Duplicate();
        GetNode<MeshInstance3D>("VIEW").MaterialOverride = mat;
        GetNode<MeshInstance3D>("VIEW").Visible = true;
        _flags.Checkpoint = OverrideInitialType;
        _flags.MaterialOutdated += UpdateMaterialsqwe;
        UpdateMaterials();
    }
    public void UpdateMaterials()
    {
        if (FinishMat == null) return;
        CheckpointFlags flags = (CheckpointFlags)Flags;
        switch (flags.Checkpoint) {
            case CHECKPOINT_TYPES.FINISHLINE:
                mat.AlbedoTexture = FinishImg;
                MaterialOverride = FinishMat;
                Deletable = false;
                break;
            case CHECKPOINT_TYPES.REPLENISHER:
                mat.AlbedoTexture = ReplenisherImg;
                MaterialOverride = ReplenisherMat;
                Deletable = true;
                break;
            default:
                mat.AlbedoTexture = CheckpointImg;
                MaterialOverride = CheckpointMat;
                Deletable = true;
                break;
        }
        return;
    }
}