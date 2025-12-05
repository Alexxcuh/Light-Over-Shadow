using Godot;
using System;

public partial class MainMenuButtons : VBoxContainer
{
    [Export] private Control MainMenu;
    [Export] private Control Levels;
    [Export] private Control contr;
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey input)
        {
            if (input.KeyLabel == Key.Escape)
            {
                MainMenu.Visible = true;
                Levels.Visible = false;
                contr.Visible = false;
            }
        }
    }
    public void Controls()
    {
        MainMenu.Visible = false;
        Levels.Visible = false;
        contr.Visible = true;
    }
    public void LevelMenu()
    {
        MainMenu.Visible = false;
        Levels.Visible = true;
        contr.Visible = false;
    }
    public void Quit()
    {
        GetTree().Quit();
    }
}
