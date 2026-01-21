using System.Collections.Generic;
using System.Text.Json;
using Godot;
using LOSUtils;
namespace LOSUtils {
    public static class CommunityLevel
    {
        public static void CheckFolder()
        {
            using var dir = DirAccess.Open("user://");
            if (!dir.DirExists("Level"))
                dir.MakeDir("Level");
        }

        public static void Save(CommunityLevelInfo Info)
        {
            CheckFolder();
            using var w = FileAccess.Open($"user://Level/{Info.Name}.AMF", FileAccess.ModeFlags.Write);
            w.StoreString(CommunityLevelInfo.Serialize(Info));
            w.Flush();
            w.Close();
        }
        public static CommunityLevelInfo Read(string LevelName)
        {
            CheckFolder();
            if (!FileAccess.FileExists($"user://Level/{LevelName}.AMF")) {
                return new CommunityLevelInfo();
            }
            using var file = FileAccess.Open($"user://Level/{LevelName}.AMF", FileAccess.ModeFlags.Read);
            string level = file.GetAsText();
            file.Close();
            return CommunityLevelInfo.Deserialize(level);
        }
    }
}