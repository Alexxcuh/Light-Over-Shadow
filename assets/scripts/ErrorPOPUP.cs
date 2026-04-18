using Godot;
using System;

public partial class ErrorPOPUP : Panel
{
    [Export]private Label title;
    [Export]private RichTextLabel body;
    public void Pressed()
    {
        Visible = false;
    }
    public void Popup(string TITLE, string BODY)
    {
        title.Text = TITLE;
        body.Text = BODY;
        Visible = true;
        GD.Print(title," ",body);
    }
}
