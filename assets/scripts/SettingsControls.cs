using Godot;
using System;

public partial class SettingsControls : Control
{
    [Export] private MainMenuButtons mmb;
    [Export] public HSlider MouseSensitivity;
    public void MouseSens(float val)
    {
        mmb.menu.Save.Settings.MouseSens = MouseSensitivity.Value;
        MouseSensitivity.GetNode<Label>("Label").Text = $"Mouse Sensitivity - {MouseSensitivity.Value}";
    }
}
