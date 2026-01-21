using Godot;
using System;

public partial class OptionMenuButton : Node
{
    int SelectedItem;
    [Export] public Godot.Collections.Dictionary Items = new()
    {
        {"2x",10},
    };
    [Export] public OptionButton Option;
    private void Select(int index)
    {
        SelectedItem = index;
        _Item_Selected(index);
    }
    private void Populate()
    {
        foreach (var item in Items)
        {
            Option.AddItem((string)item.Key);
        }
    }
    public override void _Ready()
    {
        _Item_Ready();
        Populate();
    }
    public virtual void _Item_Selected(int index)
    {
        
    }
    public virtual void _Item_Ready()
    {
        
    }
}
