
public class AreaCompCommon : Component
{
    [PropertySync(SyncConst.AllClient)]
    public DataFloatNode x = new DataFloatNode();

    [PropertySync(SyncConst.AllClient)]
    public DataFloatNode y = new DataFloatNode();

    [PropertySync(SyncConst.AllClient)]
    public DataFloatNode z = new DataFloatNode();

    public Vec3 areaPosition
    {
        get
        {
            return new Vec3(x.GetValue(), y.GetValue(), z.GetValue());
        }
    }
}
