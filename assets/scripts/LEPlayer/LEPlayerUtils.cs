using Godot;
using static LOSUtils.ObjectInfo;
namespace LOSUtils
{
    public class LEPlayerUtils
    {
        public static string PlatformPath { get; private set; } = "res://assets/scenes/Platform.tscn"; 
        public static PackedScene PlatformScene { get; private set; } = null;
        public static string CheckpointPath { get; private set; } = "res://assets/scenes/Finish.tscn"; 
        public static PackedScene CheckpointScene { get; private set; } = null;
        private static bool containsfinishline = false;
        public static void Reset()
        {
            containsfinishline = false;
        }
        public static bool ContainsFinishLine()
        {
            return containsfinishline;
        }
        /// <summary>
        /// Creates an object as a given object type in a given objects group under a given scale and position.
        /// </summary>
        /// <param name="object_type">Object Type</param>
        /// <param name="Parent">Objects Group</param>
        /// <param name="Scale">Scale</param>
        /// <param name="Position">Position</param>
        public static Node3D CreateObject(Node Parent, ObjectFlags? Flags, CommunityLevelInfo.Vector3? Scale, CommunityLevelInfo.Vector3? Position)
        {
            if (Flags == null)
            {
                GD.PrintErr("No ObjectFlags");
                return null;
            }
            Scale ??= CommunityLevelInfo.Vector3.One;
            Position ??= CommunityLevelInfo.Vector3.Zero;
            if (PlatformScene == null) PlatformScene = (PackedScene)ResourceLoader.Load(PlatformPath);
            if (CheckpointScene == null) CheckpointScene = (PackedScene)ResourceLoader.Load(CheckpointPath);
            switch (Flags.Type)
            {
                case OBJECT_TYPES.PLATFORM:
                    StaticBody3D platform = (StaticBody3D)PlatformScene.Instantiate();
                    Parent.AddChild(platform);
                    platform.Scale = new Vector3(Scale.X,Scale.Y,Scale.Z);
                    platform.Position = new Vector3(Position.X,Position.Y,Position.Z);
                    return platform;
                case OBJECT_TYPES.CHECKPOINT:
                    CheckpointFlags flags = (CheckpointFlags)Flags;
                    Finish checkpoint = (Finish)CheckpointScene.Instantiate();
                    Parent.AddChild(checkpoint);
                    checkpoint.Scale = new Vector3(Scale.X,Scale.Y,Scale.Z);
                    checkpoint.Position = new Vector3(Position.X,Position.Y,Position.Z);
                    checkpoint.ReplenishLight = flags.ReplenishAmount;
                    checkpoint.FinishLine = flags.Checkpoint == CHECKPOINT_TYPES.FINISHLINE;
                    checkpoint.replenisher = flags.Checkpoint == CHECKPOINT_TYPES.REPLENISHER;
                    if (checkpoint.FinishLine) containsfinishline = true;
                    return checkpoint;
                default:
                    return null;
            }
        }
    }
}