using Godot;
using LOSUtils;
using static LOSUtils.ObjectInfo;
public partial class CommunityLevelPlayer : Node3D
{
    [Export] public string LevelHandle = "COM_Test";
    [Export] private Player player;
    private CommunityLevelInfo LevelData;
    [Export] private Node3D LevelGroup;
    [Export] private PackedScene PlatformScene;
    [Export] private PackedScene CheckpointScene;
    public override void _Ready()
    {
        Name = LevelHandle;
        LevelData = CommunityLevel.Read(LevelHandle.Replace("COM_",""));
        player.StartPlatforms = LevelData.StartAmountPlatforms;
        var objectsNode = LevelGroup.GetNode<Node3D>("Objects");
        LEPlayerUtils.Reset();
        foreach (Object obj in LevelData.Objects)
        {
            LEPlayerUtils.CreateObject(objectsNode,obj.Flags,obj.Scale,obj.Position);
        }
        if (!LEPlayerUtils.ContainsFinishLine())
        {
            LEPlayerUtils.CreateObject(objectsNode,new CheckpointFlags(){Checkpoint = CHECKPOINT_TYPES.FINISHLINE},CommunityLevelInfo.Vector3.One,CommunityLevelInfo.Vector3.Zero);
        }
    }
}
