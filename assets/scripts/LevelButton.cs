using Godot;
using LOSUtils;

public partial class LevelButton : Panel
{
    [ExportGroup("Dependencies")]
    [Export] TextureRect Icon;
    [Export] Label _Name;
    [Export] Button Play;
    [ExportGroup("Level")]
    [Export] PackedScene AssociatedLevel;
    [Export] CompressedTexture2D LevelIcon;
    public override void _Ready()
    {
        if (LevelIcon != null) Icon.Texture = LevelIcon;
        _Name.Text = Loader.GetLevelNameFromScene(AssociatedLevel);
    }
    public void pressed()
    {
        GetTree().ChangeSceneToPacked(AssociatedLevel);
    }
}
