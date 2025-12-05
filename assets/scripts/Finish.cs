using Godot;

public partial class Finish : Area3D
{
    [Export] public bool FinishLine;
    [Export] public int ReplenishLight = 2;
    [Export] public bool replenisher = false;
    [Export] AudioStream Checkpoint = ResourceLoader.Load<AudioStream>("res://assets/visuals/checkpoint.wav");
    [Export] AudioStream Replenish = ResourceLoader.Load<AudioStream>("res://assets/visuals/replenish.wav");
    public AudioStreamPlayer3D SFX;
    public bool used = false;
    public override void _Ready()
    {
        SFX = new AudioStreamPlayer3D();
        AddChild(SFX);
    }
    public void Collided(Node3D body)
    {
        if (body is Player player && !used)
        {
            if (FinishLine || !replenisher) SFX.Stream = Checkpoint;
            if (FinishLine)
            {
                player.finished = true;
                player.Pause(1, true);
            }
            else
            {
                player.lightstuff = ReplenishLight;
                if (replenisher) SFX.Stream = Replenish;
            }
            SFX.Play();
            player.Reset += reset;
            used = true;
            if (!replenisher) player.SpawnPoint = GlobalPosition + Vector3.Up * 3f;
        }
    }
    public void reset()
    {
        used = false;
    }
}
