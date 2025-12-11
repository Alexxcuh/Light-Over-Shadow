using System.Linq;
using System.Text.Json;
using Godot;

namespace LOSUtils
{
    public static class Demo
    {
        public static void CheckFolder()
        {
            using var dir = DirAccess.Open("user://");
            if (!dir.DirExists("Demos"))
                dir.MakeDir("Demos");
        }

        public static string[] GetDemos()
        {
            using var folder = DirAccess.Open("user://Demos/");
            return folder.GetFiles();
        }
        public static void SaveDemo(Player player)
        {
            CheckFolder();
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
            CheckFolder();
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
    }
}
