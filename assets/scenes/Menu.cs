using Godot;
using System;

public partial class Menu : Node2D
{
    [Export] public PackedScene DiscordRPC;
    public override void _PhysicsProcess(double delta)
    {
        if (GetTree().Root.GetNodeOrNull<Discord>("DiscordRPC") == null)
        {
            Discord b = DiscordRPC.Instantiate<Discord>();
            b.Name = "DiscordRPC";
            GetTree().Root.AddChild(b);
            b.Owner = null;
        }
    }
}
