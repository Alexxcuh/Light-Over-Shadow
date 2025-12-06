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
}