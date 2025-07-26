
public class EntityManager : EntityManagerCommon
{
    public string primaryPid = "";

    protected override void InitComponents()
    {
        base.InitComponents();
        InitComponent<RpcComp>();
    }

    #region REGION_PLAYER

    [Rpc(RpcConst.Server, DataNodeConst.DataTypeDictionary, DataNodeConst.DataTypeDictionary)]
    public void CreatePrimaryPlayerRemote(DataDictionaryNode<string> baseProperty, DataDictionaryNode<string> compProperty)
    {
        PlayerEntity player = CreatePlayer(baseProperty, compProperty);
        if (player != null)
        {
            primaryPid = player.eid.GetValue();
        }
    }

    [Rpc(RpcConst.Server, DataNodeConst.DataTypeDictionary, DataNodeConst.DataTypeDictionary)]
    public void CreatePlayerRemote(DataDictionaryNode<string> baseProperty, DataDictionaryNode<string> compProperty)
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
