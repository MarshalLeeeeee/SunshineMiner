
public class AreaCompCommon : Component
{
    [PropertySync(SyncConst.AllClient)]
    public PropFloatNode x = new PropFloatNode();

    [PropertySync(SyncConst.AllClient)]
    public PropFloatNode y = new PropFloatNode();

    [PropertySync(SyncConst.AllClient)]
    public PropFloatNode z = new PropFloatNode();

    public Vec3 areaPosition
    {
        get
        {
            return new Vec3(x.GetValue(), y.GetValue(), z.GetValue());
        }
    }
}
