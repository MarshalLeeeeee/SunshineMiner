
public class EntityManager : EntityManagerCommon
{
    public string primaryPid = "";

    #region REGION_PLAYER

    [Rpc(RpcConst.Server, CustomTypeConst.TypeDict, CustomTypeConst.TypeDict)]
    public void CreatePrimaryPlayerRemote(CustomDict baseProperty, CustomDict compProperty)
    {
        PlayerEntity player = CreatePlayer(baseProperty, compProperty);
        if (player != null)
        {
            primaryPid = player.eid.Getter();
        }
    }

    [Rpc(RpcConst.Server, CustomTypeConst.TypeDict, CustomTypeConst.TypeDict)]
    public void CreatePlayerRemote(CustomDict baseProperty, CustomDict compProperty)
    {
        CreatePlayer(baseProperty, compProperty);
    }

    public PlayerEntity? GetPrimaryPlayer()
    {
        if (primaryPid == null)
        {
            return null;
        }
        else
        {
            return GetPlayer(primaryPid);
        }
    }

    #endregion
}
