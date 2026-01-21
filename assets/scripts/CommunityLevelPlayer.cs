using Godot;
using LOSUtils;
using System;

public partial class CommunityLevelPlayer : Node3D
{
    [Export] public string LevelHandle = "COM_Test";
    [Export] private Player player;
    private CommunityLevelInfo LevelData;
    [Export] private Node3D LevelGroup;
    [Export] private PackedScene CheckpointScene;
    [Export] private PackedScene PlatformScene;
    public override void _Ready()
    {
        Name = LevelHandle;
        LevelData = CommunityLevel.Read(LevelHandle.Replace("COM_",""));
        player.StartPlatforms = LevelData.StartAmountPlatforms;
        foreach (CommunityLevelInfo.Platform platform in LevelData.Platforms)
        {
            StaticBody3D pltfmr = (StaticBody3D)PlatformScene.Instantiate();
            LevelGroup.GetNode<Node3D>("Platforms").AddChild(pltfmr);
            pltfmr.Scale = new Vector3(platform.Scale.X,platform.Scale.Y,platform.Scale.Z);
            pltfmr.Position = new Vector3(platform.Position.X,platform.Position.Y,platform.Position.Z);
        }
        foreach (CommunityLevelInfo.Checkpoint Checkpoint in LevelData.Checkpoints)
        {
            Finish chk = (Finish)CheckpointScene.Instantiate();
            chk.replenisher = Checkpoint.Replenisher;
            chk.ReplenishLight = Checkpoint.ReplenishAmount;
            chk.FinishLine = Checkpoint.Finish;
            chk.ineditor = false;
            chk.Scale = new Vector3(Checkpoint.Scale.X,Checkpoint.Scale.Y,Checkpoint.Scale.Z);
            chk.Position = new Vector3(Checkpoint.Position.X,Checkpoint.Position.Y,Checkpoint.Position.Z);
            LevelGroup.GetNode<Node3D>("Checkpoints").AddChild(chk);
            chk.UpdateMaterials();
        }
        if (LevelData.Checkpoints.Count == 0)
        {
            Finish chk = (Finish)CheckpointScene.Instantiate();
            chk.FinishLine = true;
            chk.ineditor = false;
            chk.Scale = new Vector3(2.5f,0.5f,2.5f);
            chk.Position = new Vector3(-4.0f,0.1f,0);
            LevelGroup.GetNode<Node3D>("Checkpoints").AddChild(chk);
            chk.UpdateMaterials();
        }
    }
}
