using Godot;

public partial class Finish : Area3D
{
    [Export] private StandardMaterial3D CheckpointMat = null;
    [Export] private StandardMaterial3D ReplenisherMat = null;
    [Export] private StandardMaterial3D FinishMat = null;
    [Export] private CompressedTexture2D CheckpointImg;
    [Export] private CompressedTexture2D ReplenisherImg;
    [Export] private CompressedTexture2D FinishImg;
    [Export] public bool FinishLine;
    [Export] public int ReplenishLight = 2;
    [Export] public bool replenisher = false;
    [Export] AudioStream Checkpoint = ResourceLoader.Load<AudioStream>("res://assets/Audio/checkpoint.wav");
    [Export] AudioStream Replenish = ResourceLoader.Load<AudioStream>("res://assets/Audio/replenish.wav");
    [Export] private StandardMaterial3D bilboard;
    private StandardMaterial3D mat;
    public AudioStreamPlayer3D SFX;
    public bool used = false;
    [Export] public bool ineditor = false;
    public override void _Ready()
    {
        if (ineditor) {
            mat = (StandardMaterial3D)bilboard.Duplicate();
            GetNode<MeshInstance3D>("MeshInstance3D2").MaterialOverride = mat;
            GetNode<MeshInstance3D>("MeshInstance3D2").Visible = true;
        }
        SFX = new AudioStreamPlayer3D();
        AddChild(SFX);
        UpdateMaterials();
    }
    public void UpdateMaterials()
    {
        if (FinishMat == null) return;
        if (FinishLine) {
            if (ineditor) {
                mat.AlbedoTexture = FinishImg;
            }
            GetNode<MeshInstance3D>("MeshInstance3D").MaterialOverride = FinishMat;
        }
        else if (replenisher) {
            if (ineditor) {
                mat.AlbedoTexture = ReplenisherImg;
            }
            GetNode<MeshInstance3D>("MeshInstance3D").MaterialOverride = ReplenisherMat;
        }
        else {
            if (ineditor) {
                mat.AlbedoTexture = CheckpointImg;
            }
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
