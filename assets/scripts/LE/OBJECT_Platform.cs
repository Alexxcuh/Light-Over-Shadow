using LOSUtils;
using static LOSUtils.ObjectInfo;

public partial class OBJECT_Platform : Editor_Object
{
    private PlatformFlags _flags = new PlatformFlags();
    [EditorVisible]
    public override ObjectFlags Flags
    {
        get => _flags;
        set => _flags = (PlatformFlags)value;
    }

}