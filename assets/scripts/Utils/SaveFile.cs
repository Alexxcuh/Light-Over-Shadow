using System.Collections.Generic;
using System.Text.Json;
using Godot;
using LOSUtils;

public static class SaveFile
{
    public static void CheckFolder()
    {
        using var dir = DirAccess.Open("user://");
        if (!dir.DirExists("Save"))
            dir.MakeDir("Save");
    }

    public static void Save(Player player)
    {
        CheckFolder();

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
        CheckFolder();

        if (!FileAccess.FileExists("user://Save/player.AST"))
            return new LevelData();

        using var file = FileAccess.Open("user://Save/player.AST", FileAccess.ModeFlags.Read);
        var json = file.GetAsText();
        file.Close();

        return JsonSerializer.Deserialize<LevelData>(json) ?? new LevelData();
    }
}
