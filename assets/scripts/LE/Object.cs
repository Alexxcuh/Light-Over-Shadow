using System;
using System.Xml.Serialization;
using static LOSUtils.CommunityLevelInfo;

namespace LOSUtils {
    public static class ObjectInfo
    {
        public partial class Object
        {
            public int ID { get; set; } = 0;
            public Vector3 Position { get; set; } = Vector3.Zero;
            public Vector3 Scale { get; set; } = Vector3.One;
            [XmlElement("PlatformFlags", typeof(PlatformFlags))]
            [XmlElement("CheckpointFlags", typeof(CheckpointFlags))]
            public ObjectFlags Flags { get; set; }
        }

        [XmlInclude(typeof(PlatformFlags))]
        [XmlInclude(typeof(CheckpointFlags))]
        public abstract class ObjectFlags
        {
            [EditorVisible]
            public abstract OBJECT_TYPES Type { get; }
        }
        public class PlatformFlags : ObjectFlags
        {
            [EditorVisible]
            public override OBJECT_TYPES Type => OBJECT_TYPES.PLATFORM;

            internal void Clear()
            {
                throw new NotImplementedException();
            }
        }
        public class CheckpointFlags : ObjectFlags
        {
            public event EventHandler MaterialOutdated;
            [EditorVisible]
            public override OBJECT_TYPES Type => OBJECT_TYPES.CHECKPOINT;
            private CHECKPOINT_TYPES chckpoint = CHECKPOINT_TYPES.CHECKPOINT;
            [EditorVisible]
            public CHECKPOINT_TYPES Checkpoint
            {
                get => chckpoint;
                set => SetCheckpoint(value);
            }
            private void SetCheckpoint(CHECKPOINT_TYPES val)
            {
                chckpoint = val;
                if (MaterialOutdated != null) MaterialOutdated.Invoke(this,null);
            }
            public void Clear()
            {
                MaterialOutdated = null;
            }
            [EditorVisible]
            public int ReplenishAmount { get; set; } = 0;
        }
    }
}