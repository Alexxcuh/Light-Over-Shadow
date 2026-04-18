using Godot;
using System;
using LOSUtils;
public partial class LevelSecondary : VBoxContainer
{
    private SaveData Save;
    [Export] PackedScene CLP;
    [Export] PackedScene lb;
    public override void _Ready()
    {
        Save = SaveFile.Read();
        if (DirAccess.Open("user://").DirExists("Level")){
            string[] files = DirAccess.GetFilesAt("user://Level");
            Label tm = new Label();
            tm.Text="Community Levels";
            tm.HorizontalAlignment = HorizontalAlignment.Center;
            tm.VerticalAlignment = VerticalAlignment.Center;
            AddChild(tm);
            AddChild(new HSeparator());
            foreach (string level in files)
            {
                CommunityLevelInfo temp = CommunityLevel.Read(level.Split(".")[0]);
                LevelButton tmp = (LevelButton)lb.Instantiate();
                tmp.CommunityLevel = true;
                tmp.AssociatedLevel = CLP;
                tmp.LevelHandle = $"COM_{level.Split(".")[0]}";
                tmp.LevelName = temp.Name;
                AddChild(tmp);
                tmp._Ready();
            }
        }
        var kids = GetChildren();
        foreach (Node item in kids)
        {
            if (item is LevelButton but) {
                string name;
                if (but.CommunityLevel) name = but.LevelHandle;
                else name = but.AssociatedLevel.ResourcePath.Split("/")[^1][..^5];
                if (Save.Levels.ContainsKey(name))
                {
                    float time = Save.Levels[name].Time;
                    but.GetNode<Label>("PB").Text = $"PB - {(int)(time / 60 % 60):00}:{(int)(time % 60):00}.{(int)(time * 100 % 100):00}";;
                } else
                {
                    but.GetNode<Label>("PB").Visible = false;
                    continue;
                }
            }
        }
        base._Ready();
    }
}
