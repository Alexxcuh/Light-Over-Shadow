using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Godot;
namespace LOSUtils
{
    public class MathA
    {
        public static int Compare(int[] ticks, int tick)
        {
            int temp = 0;
            foreach (float t in ticks)
            {
                temp += t > tick ? 0:1; 
            }
            return temp;
        }
    }
    public class Loader
    {
        public static string GetLevelNameFromScene(PackedScene Level)
        {
            var state = Level.GetState();

            int propertyCount = state.GetNodePropertyCount(0);

            for (int i = 0; i < propertyCount; i++)
            {
                string propName = state.GetNodePropertyName(0, i);
                if (propName == "LevelName")
                {
                    GD.Print(i,propertyCount);
                    return (string)state.GetNodePropertyValue(0, i);
                }
            }

            return "Unnamed";
        }
    }

    //File Shtuff
    public static class SaveFile
    {
        public static void CheckSaveFolder()
        {
            using var dir = DirAccess.Open("user://");
            if (!dir.DirExists("Save"))
                dir.MakeDir("Save");
        }

        public static void Save(Player player)
        {
            CheckSaveFolder();

            LevelData data;

            if (FileAccess.FileExists("user://Save/player.AST"))
            {
                using var r = FileAccess.Open("user://Save/player.AST", FileAccess.ModeFlags.Read);
                var txt = r.GetAsText();
                data = JsonSerializer.Deserialize<LevelData>(txt) ?? new LevelData();
            }
            else
            {
                data = new LevelData();
            }

            data.Levels ??= new Dictionary<string, LevelInfo>();

            data.Levels[player.RootScene.Name] = new LevelInfo
            {
                Time = player.Time
            };

            using var w = FileAccess.Open("user://Save/player.AST", FileAccess.ModeFlags.Write);
            w.StoreString(JsonSerializer.Serialize(data));
            w.Flush();
            w.Close();
        }

        public static LevelData Read()
        {
            CheckSaveFolder();

            if (!FileAccess.FileExists("user://Save/player.AST"))
                return new LevelData();

            using var file = FileAccess.Open("user://Save/player.AST", FileAccess.ModeFlags.Read);
            var json = file.GetAsText();
            file.Close();

            return JsonSerializer.Deserialize<LevelData>(json) ?? new LevelData();
        }
    }
    public static class Demo
    {
        public static void CheckDemoFolder()
        {
            using var dir = DirAccess.Open("user://");
            if (!dir.DirExists("Demos"))
                dir.MakeDir("Demos");
        }

        public static string[] GetDemos()
        {
            using var folder = DirAccess.Open("user://Demos/");
            string[] list = [.. folder.GetFiles().Where(s => s.EndsWith(".ADT"))];
            return list;
        }
        public static void SaveDemo(Player player)
        {
            CheckDemoFolder();
            using var file = FileAccess.Open($"user://Demos/{player.RootScene.Name}.ADT", FileAccess.ModeFlags.WriteRead);
            file.StoreString(JsonSerializer.Serialize(new
            {
                Level = player.RootScene.Name.ToString(),
                Positions = player.Positions.Select(v => new float[] { v.X, v.Y, v.Z }).ToArray(),
                Times = player.Times,
                Platforms = player.PlatformTicks.Select(v => new float[] { v.X, v.Y, v.Z, v.W }).ToArray(),
            }));
            file.Flush();
            file.Close();
        }

        public static (Godot.Collections.Array<Vector4> Platforms, float[] times, string Level, Godot.Collections.Array<Vector3> Positions, Godot.Collections.Array<Vector3> vels) LoadDemo(string Name)
        {
            CheckDemoFolder();
            using var file = FileAccess.Open($"user://Demos/{Name}.ADT", FileAccess.ModeFlags.Read);

            var jsonText = file.GetAsText();
            file.Close();
            var data = JsonSerializer.Deserialize<DemoData>(jsonText, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var positions = new Godot.Collections.Array<Vector3>();
            foreach (var a in data.Positions)
                positions.Add(new Vector3(a[0], a[1], a[2]));
            var platforms = new Godot.Collections.Array<Vector4>();
            foreach (var a in data.Platforms)
                platforms.Add(new Vector4(a[0], a[1], a[2], a[3]));
            var vels = new Godot.Collections.Array<Vector3>();
            foreach (var a in data.Velocities)
                vels.Add(new Vector3(a[0], a[1], a[2]));
            return (platforms ,data.Times, data.Level, positions, vels);
        }
        public static string ReadName(string Name)
        {
            CheckDemoFolder();
            using var file = FileAccess.Open($"user://Demos/{Name}.ADT", FileAccess.ModeFlags.Read);

            var jsonText = file.GetAsText();
            file.Close();
            var data = JsonSerializer.Deserialize<DemoData>(jsonText, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return data.Level;
        }
    }
}