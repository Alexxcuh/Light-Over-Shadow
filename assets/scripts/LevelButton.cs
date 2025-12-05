using Godot;

public partial class LevelButton : Panel
{
    [ExportGroup("Dependencies")]
    [Export] TextureRect Icon;
    [Export] Label _Name;
    [Export] Button Play;
    [ExportGroup("Level")]
    [Export] PackedScene AssociatedLevel;
    [Export] CompressedTexture2D LevelIcon;
    [Export] string LevelName;
    public override void _Ready()
    {
        _Name.Text = LevelName;
        if (LevelIcon != null) Icon.Texture = LevelIcon;
    }
    public void pressed()
    {
        GetTree().ChangeSceneToPacked(AssociatedLevel);
    }
}
