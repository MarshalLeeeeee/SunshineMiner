using System.Collections;
using System.Collections.Generic;

public class AreaCompBase : Component
{
    [PropertySync(SyncConst.AllClient)]
    public CustomFloat x = new CustomFloat();

    [PropertySync(SyncConst.AllClient)]
    public CustomFloat y = new CustomFloat();

    [PropertySync(SyncConst.AllClient)]
    public CustomFloat z = new CustomFloat();

    public Vec3 areaPosition
    {
        get
        {
            return new Vec3(x.Getter(), y.Getter(), z.Getter());
        }
    }
}
