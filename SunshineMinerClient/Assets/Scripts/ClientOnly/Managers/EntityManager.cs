
public class EntityManager : EntityManagerCommon
{
    public string primaryPid = "";

    protected override void InitComponents()
    {
        base.InitComponents();
        InitComponent<RpcComp>();
    }

    #region REGION_PLAYER

    [Rpc(RpcConst.Server, SyncDataConst.DataTypeDictionary, SyncDataConst.DataTypeDictionary)]
    public void CreatePrimaryPlayerRemote(SyncDataDictionaryNode<string> baseProperty, SyncDataDictionaryNode<string> compProperty)
    {
        PlayerEntity player = CreatePlayer(baseProperty, compProperty);
        if (player != null)
        {
            primaryPid = player.eid.GetValue();
        }
    }

    [Rpc(RpcConst.Server, SyncDataConst.DataTypeDictionary, SyncDataConst.DataTypeDictionary)]
    public void CreatePlayerRemote(SyncDataDictionaryNode<string> baseProperty, SyncDataDictionaryNode<string> compProperty)
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
