using Godot;
using System;

public partial class Render : Node3D
{
    [Export] private SubViewport viewport;
    private void _Render()
    {
        Image img = viewport.GetTexture().GetImage();
        img.SavePng("user://RENDER.PNG");
    }
}
