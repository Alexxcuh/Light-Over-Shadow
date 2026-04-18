using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Godot;
using static LOSUtils.ObjectInfo;
namespace LOSUtils
{
    public class ObjectUtils
    {
        public static string PlatformPath { get; private set; } = "res://assets/scenes/LevelEditor/PlatformObject.tscn"; 
        public static PackedScene PlatformScene { get; private set; } = null;
        public static string CheckpointPath { get; private set; } = "res://assets/scenes/LevelEditor/CheckpointObject.tscn"; 
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
        public static Editor_Object CreateObject(Node Parent, ObjectFlags? Flags, CommunityLevelInfo.Vector3? Scale, CommunityLevelInfo.Vector3? Position)
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
                    OBJECT_Platform platform = (OBJECT_Platform)PlatformScene.Instantiate();
                    Parent.AddChild(platform);
                    platform.Scale = new Vector3(Scale.X,Scale.Y,Scale.Z);
                    platform.Position = new Vector3(Position.X,Position.Y,Position.Z);
                    platform.Flags = Flags;
                    return platform;
                case OBJECT_TYPES.CHECKPOINT:
                    OBJECT_Checkpoint checkpoint = (OBJECT_Checkpoint)CheckpointScene.Instantiate();
                    Parent.AddChild(checkpoint);
                    checkpoint.Scale = new Vector3(Scale.X,Scale.Y,Scale.Z);
                    checkpoint.Position = new Vector3(Position.X,Position.Y,Position.Z);
                    checkpoint.Flags = Flags;
                    checkpoint.UpdateMaterials();
                    if (!containsfinishline) {
                        CheckpointFlags flags = (CheckpointFlags)Flags;
                        if (flags.Checkpoint == CHECKPOINT_TYPES.FINISHLINE) containsfinishline = true;
                    }
                    return checkpoint;
                default:
                    return null;
            }
        }
        public static Object SerializeObject(Editor_Object obj)
        {
            return new Object()
            {
                Position = new CommunityLevelInfo.Vector3(obj.Position.X,obj.Position.Y,obj.Position.Z),
                Scale = new CommunityLevelInfo.Vector3(obj.Scale.X,obj.Scale.Y,obj.Scale.Z),
                Flags = obj.Flags
            };
        }
        public static void SetMemberValue(MemberInfo member, object obj, object value)
        {
            if (member is FieldInfo f)
                f.SetValue(obj, value);

            else if (member is PropertyInfo p && p.CanWrite)
                p.SetValue(obj, value);
            
        }
        public static List<MemberInfo> GetEditorMembers(object obj)
        {
            var type = obj.GetType();

            var fields = type.GetFields(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance
            ).Cast<MemberInfo>();

            var props = type.GetProperties(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance
            ).Cast<MemberInfo>();

            return fields
                .Concat(props)
                .Where(m => m.GetCustomAttribute<EditorVisibleAttribute>() != null)
                .ToList();
        }
        public static object GetMemberValue(MemberInfo member, object obj)
        {
            if (member is PropertyInfo prop)
            {
                return prop.GetValue(obj);
            }
            else if (member is FieldInfo field)
            {
                return field.GetValue(obj);
            }

            return null;
        }
    }
}