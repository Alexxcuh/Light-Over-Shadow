using Godot;
using System;
using LOSUtils;
public partial class Menu : Node2D
{
    [Export] public PackedScene DiscordRPC;
    public SaveData Save;
    public override void _Ready()
    {
        Discord drpc = GetTree().Root.GetNodeOrNull<Discord>("DiscordRPC");
        if (drpc != null) drpc.UpdatePresence("On Menu");
        Save = SaveFile.Read();
    }
    public override void _PhysicsProcess(double delta)
    {
        Engine.TimeScale = 1.0f;
        if (GetTree().Root.GetNodeOrNull<Discord>("DiscordRPC") == null)
        {
            Discord b = DiscordRPC.Instantiate<Discord>();
            b.Name = "DiscordRPC";
            GetTree().Root.AddChild(b);
            b.Owner = null;
        }
    }
}
