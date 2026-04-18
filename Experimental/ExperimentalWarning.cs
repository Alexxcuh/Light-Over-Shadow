using Godot;
using System;

public partial class ExperimentalWarning : Panel
{
    public override void _Ready()
    {
        if (LOSGlobals.Experimental) Visible = true;
    }
}
