using Godot;

public partial class Finish : Area3D
{
    [Export] private StandardMaterial3D CheckpointMat = null;
    [Export] private StandardMaterial3D ReplenisherMat = null;
    [Export] private StandardMaterial3D FinishMat = null;
    [Export] public bool FinishLine;
    [Export] public int ReplenishLight = 2;
    [Export] public bool replenisher = false;
    [Export] AudioStream Checkpoint = ResourceLoader.Load<AudioStream>("res://assets/Audio/checkpoint.wav");
    [Export] AudioStream Replenish = ResourceLoader.Load<AudioStream>("res://assets/Audio/replenish.wav");
    public AudioStreamPlayer3D SFX;
    public bool used = false;
    public override void _Ready()
    {
        SFX = new AudioStreamPlayer3D();
        AddChild(SFX);
        UpdateMaterials();
    }
    public void UpdateMaterials()
    {
        if (FinishMat == null) return;
        if (FinishLine) {
            GetNode<MeshInstance3D>("MeshInstance3D").MaterialOverride = FinishMat;
        }
        else if (replenisher) {
            GetNode<MeshInstance3D>("MeshInstance3D").MaterialOverride = ReplenisherMat;
        }
        else {
            GetNode<MeshInstance3D>("MeshInstance3D").MaterialOverride = CheckpointMat;
        }
        return;
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
