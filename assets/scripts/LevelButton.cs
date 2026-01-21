using Godot;

public partial class LevelButton : Panel
{
    [ExportGroup("Dependencies")]
    [Export] TextureRect Icon;
    [Export] Label _Name;
    [Export] Button Play;
    [ExportGroup("Level")]
    [Export] public PackedScene AssociatedLevel;
    [Export] CompressedTexture2D LevelIcon;
    [Export] public string LevelName;
    [ExportSubgroup("Community")]
    [Export] public bool CommunityLevel;
    [Export] public string LevelHandle;
    public override void _Ready()
    {
        _Name.Text = LevelName;
        if (LevelIcon != null) Icon.Texture = LevelIcon;
    }
    public void pressed()
    {
        if (!CommunityLevel) {
            GetTree().ChangeSceneToPacked(AssociatedLevel);
        } else
        {
            CommunityLevelPlayer CLP = (CommunityLevelPlayer)AssociatedLevel.Instantiate();
            CLP.LevelHandle = LevelHandle;
            GetTree().Root.AddChild(CLP);
            Node2D cur = (Node2D)GetTree().CurrentScene;
            GetTree().CurrentScene = CLP;
            cur.QueueFree();
        }
    }
}
