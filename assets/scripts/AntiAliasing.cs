using Godot;
using System;

public partial class AntiAliasing : OptionMenuButton
{
    public override void _Item_Selected(int index)
    {
        GD.Print("YO");
        GD.Print(ProjectSettings.GetSetting("rendering/anti_aliasing/quality/msaa_3d"));
    }
}
