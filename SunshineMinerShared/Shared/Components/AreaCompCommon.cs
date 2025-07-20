
public class AreaCompCommon : Component
{
    [PropertySync(SyncConst.AllClient)]
    public SyncDataFloatNode x = new SyncDataFloatNode();

    [PropertySync(SyncConst.AllClient)]
    public SyncDataFloatNode y = new SyncDataFloatNode();

    [PropertySync(SyncConst.AllClient)]
    public SyncDataFloatNode z = new SyncDataFloatNode();

    public Vec3 areaPosition
    {
        get
        {
            return new Vec3(x.GetValue(), y.GetValue(), z.GetValue());
        }
    }
}
