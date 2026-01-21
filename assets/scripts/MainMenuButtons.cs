using Godot;
using LOSUtils;
using System;
using System.Text.Json;

public partial class MainMenuButtons : VBoxContainer
{
    [Export] private Control MainMenu;
    [Export] private Control Levels;
    [Export] private Control contr;
    [Export] private SettingsControls settings;
    [Export] public Menu menu;
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey input)
        {
            if (input.KeyLabel == Key.Escape && input.Pressed && !input.Echo)
            {
                // UPDATE SETTINGS
                SaveFile.Update(menu.Save);
                //menu.Save.Settings.MouseSens = settings.MouseSensitivity.Value;
                MainMenu.Visible = true;
            }
        }
    }
    public void Demo()
    {
        GetTree().ChangeSceneToFile("res://assets/scenes/SaveWatch.tscn");
    }
    public void LevelEditor()
    {
        GetTree().ChangeSceneToFile("res://assets/scenes/LevelEditor.tscn");
    }
    public void Settings()
    {
        settings.Visible = true;
        settings.MouseSensitivity.Value = menu.Save.Settings.MouseSens;
        settings.MouseSens(0);
    }
    public void Controls()
    {
        contr.Visible = true;
    }
    public void LevelMenu()
    {
        Levels.Visible = true;
    }
    public void Quit()
    {
        GetTree().Quit();
    }
}
