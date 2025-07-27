
public class PlayerEntity : PlayerEntityCommon
{
    public PlayerEntity() : base() { }
    public PlayerEntity(string eid) : base(eid) { }

    protected override void InitComponents()
    {
        base.InitComponents();
        InitComponent<RpcComp>();
        InitComponent<PropComp>();
        InitComponent<PrefabComp>();
        // InitComponent<AreaComp>();
    }
}
