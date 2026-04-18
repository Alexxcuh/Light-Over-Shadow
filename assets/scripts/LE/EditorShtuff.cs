using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class EditorVisibleAttribute : Attribute
{
    public string DisplayName { get; }

    public EditorVisibleAttribute(string displayName = null)
    {
        DisplayName = displayName;
    }
}

public enum Editor_InspectTypes
{
    OBJECT = 0,
    FLAGS = 0,
}