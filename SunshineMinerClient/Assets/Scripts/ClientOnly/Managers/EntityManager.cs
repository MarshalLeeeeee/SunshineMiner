
public class EntityManager : EntityManagerCommon
{
    public string primaryPid = "";

    protected override void InitComponents()
    {
        base.InitComponents();
        InitComponent<RpcComp>();
    }

    #region REGION_PLAYER

    [Rpc(RpcConst.Server, DataNodeConst.DataTypeDictionary)]
    public void CreatePrimaryPlayerRemote(DataDictionaryNode<string> info)
    {
        PlayerEntity player = CreatePlayer(info);
        if (player != null)
        {
            primaryPid = player.eid.GetValue();
        }
    }

    [Rpc(RpcConst.Server, DataNodeConst.DataTypeDictionary)]
    public void CreatePlayerRemote(DataDictionaryNode<string> info)
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
