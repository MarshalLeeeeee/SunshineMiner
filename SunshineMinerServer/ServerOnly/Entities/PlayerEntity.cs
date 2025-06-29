
public class PlayerEntity : PlayerEntityCommon
{
    public Guid proxyId;

    public PlayerEntity() : base() { }
    public PlayerEntity(string eid) : base(eid) { }

    protected override void InitComponents()
    {
        base.InitComponents();
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
        msgOwn.arg = SerializeProperty(SyncConst.OwnClient);
        Game.Instance.gate.RpcToOwnClient(eid.Getter(), msgOwn);

        Msg msgOther = new Msg("EntityManager", "CreatePlayerRemote");
        msgOther.arg = SerializeProperty(SyncConst.AllClient);
        Game.Instance.gate.RpcToOtherClient(eid.Getter(), msgOther);
    }

    public void SyncByOthers()
    {
        foreach (PlayerEntity player in Game.Instance.entityManager.GetOtherPlayer(eid.Getter()))
        {
            Msg msgOther = new Msg("EntityManager", "CreatePlayerRemote");
            msgOther.arg = player.SerializeProperty(SyncConst.AllClient);
            Game.Instance.gate.RpcToOwnClient(eid.Getter(), msgOther);
        }
    }

    #endregion
}
