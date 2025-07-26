
public class AccountManager : Manager
{
    private Dictionary<string, string> account2player = new Dictionary<string, string>();

    protected override void InitComponents()
    {
        base.InitComponents();
        InitComponent<RpcComp>();
    }

    /*
     * Authorize account
     * TODO
     */
    private static bool CheckAccount(string account, string password)
    {
        return true;
    }

    [Rpc(RpcConst.AnyClient, PropNodeConst.DataTypeString, PropNodeConst.DataTypeString)]
    public void LoginRemote(PropStringNode account, PropStringNode password, Proxy proxy)
    {
        string accountStr = account.GetValue();
        string passwordStr = password.GetValue();
        Login(accountStr, passwordStr, proxy);
    }

    /*
     * Player login and create player entity if authorized
     */
    public void Login(string account, string password, Proxy proxy)
    {
        Msg msg = new Msg("Gate", "LoginResRemote");
        if (!CheckAccount(account, password))
        {
            msg.arg.Add(new PropBoolNode(false));
            Game.Instance.gate.AppendSendMsg(proxy, msg);
        }
        else {
            PlayerEntity? player = null;
            if (!account2player.ContainsKey(account))
            {
                Guid eid = Guid.NewGuid();
                player = Game.Instance.entityManager.CreatePlayer(eid.ToString());
                if (player != null)
                {
                    account2player[account] = player.eid.GetValue();
                }
            }
            else
            {
                player = Game.Instance.entityManager.GetPlayer(account2player[account]);
            }
            msg.arg.Add(new PropBoolNode(player != null));
            Game.Instance.gate.AppendSendMsg(proxy, msg);

            if (player != null)
            {
                proxy.eid = account2player[account];
                player.UpdateProxy(proxy.pid);
                player.SyncSelfToAll();
                player.SyncByOthers();
            }
        }
    }
}

