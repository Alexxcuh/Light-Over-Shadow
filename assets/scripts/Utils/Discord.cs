using Godot;
using System;
using DiscordRPC;
using DiscordRPC.Logging;
public partial class Discord : Node
{
    public const string DISCORD_APP_ID = "862685287490256937";
    public DiscordRpcClient client;
    private DateTime sessionStart;
    public override void _Ready()
    {
        base._Ready();
        sessionStart = DateTime.UtcNow;
        client = new DiscordRpcClient(DISCORD_APP_ID)
        {
            Logger = new ConsoleLogger(LogLevel.Info, true)
        };
        client.Initialize();
        UpdatePresence("On Menu");
    }
    public void UpdatePresence(string details, string state="")
    {
        if (client == null) return;

        client.SetPresence(new RichPresence()
        {
            Details = details,
            State = state,
            Timestamps = new Timestamps()
            {
                Start = sessionStart
            },
            Assets = new Assets()
            {
                LargeImageKey = "my_large_image"
            }
        });
    }
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        client.Dispose();
    }
}
