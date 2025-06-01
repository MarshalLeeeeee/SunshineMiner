using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayerEntity : PlayerEntityBase
{
    public Guid proxyId;

    public PlayerEntity() : base() { }
    public PlayerEntity(string eid) : base(eid) { }

    #region REGION_PROXY

    public void UpdateProxy(Guid proxyId_)
    {
        Game.Instance.gate.RemoveProxy(proxyId);
        proxyId = proxyId_;

        SyncSelf();
    }

    #endregion

    #region REGION_PLAYER_SYNC

    public void SyncSelf()
    {
        Msg msgOwn = new Msg(eid.Getter(), "EntityManager", "CreatePrimaryPlayerRemote");
        msgOwn.arg = SerializeProperty(SyncConst.OwnClient);
        Game.Instance.gate.RpcToOwnClient(eid.Getter(), msgOwn);

        Msg msgOther = new Msg(eid.Getter(), "EntityManager", "CreatePlayerRemote");
        msgOther.arg = SerializeProperty(SyncConst.AllClient);
        Game.Instance.gate.RpcToOtherClient(eid.Getter(), msgOther);
    }

    public void SyncOthers()
    {
        foreach (PlayerEntity player in Game.Instance.entityManager.GetOtherPlayer(eid.Getter()))
        {
            Msg msgOther = new Msg(eid.Getter(), "EntityManager", "CreatePlayerRemote");
            msgOther.arg = player.SerializeProperty(SyncConst.AllClient);
            Game.Instance.gate.RpcToOwnClient(eid.Getter(), msgOther);
        }
    }

    #endregion
}
