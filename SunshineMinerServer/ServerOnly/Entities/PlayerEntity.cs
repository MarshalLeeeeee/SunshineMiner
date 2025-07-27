
public class PlayerEntity : PlayerEntityCommon
{
    public Guid proxyId;

    public PlayerEntity() : base() { }
    public PlayerEntity(string eid) : base(eid) { }

    protected override void InitComponents()
    {
        base.InitComponents();
        InitComponent<RpcComp>();
        InitComponent<PropComp>();
        InitComponent<AreaComp>();
    }

    #region REGION_PROXY

    public void UpdateProxy(Guid proxyId_)
    {
        Game.Instance.gate.RemoveProxy(proxyId);
        proxyId = proxyId_;
    }

    #endregion

    #region REGION_PLAYER_SYNC

    public void SyncSelfToAll()
    {
        Msg msgOwn = new Msg("EntityManager", "CreatePrimaryPlayerRemote");
        msgOwn.arg.Add(SerializeWithSyncType(SyncConst.OwnClient));
        Game.Instance.gate.RpcToOwnClient(eid.GetValue(), msgOwn);

        Msg msgOther = new Msg("EntityManager", "CreatePlayerRemote");
        msgOther.arg.Add(SerializeWithSyncType(SyncConst.AllClient));
        Game.Instance.gate.RpcToOtherClient(eid.GetValue(), msgOther);
    }

    public void SyncByOthers()
    {
        foreach (PlayerEntity player in Game.Instance.entityManager.GetOtherPlayer(eid.GetValue()))
        {
            Msg msgOther = new Msg("EntityManager", "CreatePlayerRemote");
            msgOther.arg.Add(player.SerializeWithSyncType(SyncConst.AllClient));
            Game.Instance.gate.RpcToOwnClient(eid.GetValue(), msgOther);
        }
    }

    #endregion
}
