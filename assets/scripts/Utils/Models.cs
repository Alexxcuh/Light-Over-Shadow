using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace LOSUtils
{
    public class SettingsData
    {
        public double MouseSens { get; set; } = 3f;
    }
    public class SaveData
    {
        public SettingsData Settings { get; set; } = new SettingsData();
        public System.Collections.Generic.Dictionary<string, LevelInfo> Levels { get; set; } = [];
    }
    public class LevelInfo
    {
        public float Time { get; set; } = 0.0f;
    }
    public partial class CommunityLevelInfo
    {
        public partial class Vector3
        {
            public float X { get; set; } = 0;
            public float Y { get; set; } = 0;
            public float Z { get; set; } = 0;
            public Vector3() { }
            public Vector3(float x, float y, float z)
            {
                X = x; Y = y; Z = z;
            }
            public static Vector3 Zero => new Vector3(0, 0, 0);
            public static Vector3 One => new Vector3(1, 1, 1);
        }
        public string Version { get; set; } = "0.0.0";
        public int StartAmountPlatforms { get; set; } = 0;
        public string Name { get; set; } = "Test";
        public List<Platform> Platforms { get; set; } = [];
        public List<Checkpoint> Checkpoints { get; set; } = [];
        public partial class Platform
        {
            public Vector3 Position { get; set; } = Vector3.Zero;
            public Vector3 Scale { get; set; } = Vector3.One;
        }
        public partial class Checkpoint
        {
            public Vector3 Position { get; set; } = Vector3.Zero;
            public Vector3 Scale { get; set; } = Vector3.One;
            public bool Replenisher { get; set; } = false;
            public int ReplenishAmount { get; set; } = 0;
            public bool Finish { get; set; } = false;
        }
        public static string Serialize(CommunityLevelInfo info)
        {
            var serializer = new XmlSerializer(typeof(CommunityLevelInfo));
            using var writer = new StringWriter();
            serializer.Serialize(writer, info);
            return writer.ToString();
        }
        public static CommunityLevelInfo Deserialize(string info)
        {
            var serializer = new XmlSerializer(typeof(CommunityLevelInfo));
            using var reader = new StringReader(info);
            return (CommunityLevelInfo)serializer.Deserialize(reader);
        }
    }

    public class DemoData
        {
            public string Level { get; set; } = "Level1";
            public float[][] Positions { get; set; } = new float[0][];
            public float[] Times { get; set; } = new float[0];
            public float[][] Platforms { get; set; } = new float[0][];
            public float[][] Velocities {get; set;} = new float[0][];
        }
}