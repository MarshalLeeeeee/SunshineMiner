
public class EntityManager : EntityManagerCommon
{
    public string primaryPid = "";

    protected override void InitComponents()
    {
        base.InitComponents();
        InitComponent<RpcComp>();
    }

    #region REGION_PLAYER

    [Rpc(RpcConst.Server, PropNodeConst.TypeStringDictionary)]
    public void CreatePrimaryPlayerRemote(PropStringDictionaryNode info)
    {
        PlayerEntity player = CreatePlayer(info);
        if (player != null)
        {
            primaryPid = player.eid.GetValue();
        }
    }

    [Rpc(RpcConst.Server, PropNodeConst.TypeStringDictionary)]
    public void CreatePlayerRemote(PropStringDictionaryNode info)
    {
        CreatePlayer(info);
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
