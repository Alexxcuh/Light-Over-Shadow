namespace LOSUtils
{
    public class LevelData
    {
        public System.Collections.Generic.Dictionary<string, LevelInfo> Levels { get; set; } = [];
    }
    public class LevelInfo
    {
        public float Time { get; set; } = 0.0f;
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