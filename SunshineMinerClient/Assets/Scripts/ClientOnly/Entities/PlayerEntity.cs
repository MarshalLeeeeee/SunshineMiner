
public class PlayerEntity : PlayerEntityCommon
{
    public PlayerEntity() : base() { }
    public PlayerEntity(string eid) : base(eid) { }

    protected override void InitComponents()
    {
        base.InitComponents();
        InitComponent<PrefabComp>();
        InitComponent<AreaComp>();
        InitComponent<RpcComp>();
    }
}
