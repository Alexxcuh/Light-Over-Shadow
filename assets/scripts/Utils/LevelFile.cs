// using System.Collections.Generic;
// using System.Text.Json;
// using Godot;
// using LOSUtils;
// namespace LOSUtils {
//     public static class LevelFile
//     {
//         public static void CheckFolder()
//         {
//             using var dir = DirAccess.Open("user://");
//             if (!dir.DirExists("Levels"))
//                 dir.MakeDir("Levels");
//         }

//         public static void Save(string Level)
//         {
//             CheckFolder();

//             SaveData data;

//             if (FileAccess.FileExists("user://Levels/player.AMF"))
//             {
//                 using var r = FileAccess.Open("user://Levels/player.AMF", FileAccess.ModeFlags.Read);
//                 var txt = r.GetAsText();
//                 data = JsonSerializer.Deserialize<SaveData>(txt) ?? new SaveData();
//             }
//             else
//             {
//                 data = new SaveData();
//             }

//             data.Levels ??= new Dictionary<string, LevelInfo>();

//             data.Levels[player.RootScene.Name] = new LevelInfo
//             {
//                 Time = player.Time
//             };

//             using var w = FileAccess.Open("user://Save/player.AMF", FileAccess.ModeFlags.Write);
//             w.StoreString(JsonSerializer.Serialize(data));
//             w.Flush();
//             w.Close();
//         }
//         public static void Update(SaveData Save)
//         {
//             CheckFolder();
//             if (FileAccess.FileExists("user://Save/player.AMF"))
//             {
//                 using var w = FileAccess.Open("user://Save/player.AMF", FileAccess.ModeFlags.Write);
//                 w.StoreString(JsonSerializer.Serialize(Save));
//                 w.Flush();
//                 w.Close();
//             }
//         }
//         public static SaveData Read()
//         {
//             CheckFolder();

//             if (!FileAccess.FileExists("user://Save/player.AST"))
//                 return new SaveData();

//             using var file = FileAccess.Open("user://Save/player.AST", FileAccess.ModeFlags.Read);
//             var json = file.GetAsText();
//             file.Close();

//             return JsonSerializer.Deserialize<SaveData>(json) ?? new SaveData();
//         }
//     }
// }