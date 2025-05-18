using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class AccountManager : Manager
{
    private Dictionary<string, string> account2player = new Dictionary<string, string>();

    public AccountManager(string eid) : base(eid) { }

    /*
     * Authorize account
     * TODO
     */
    private static bool CheckAccount(string account, string password)
    {
        return true;
    }

    [Rpc(RpcConst.AnyClient, CustomTypeConst.TypeString, CustomTypeConst.TypeString)]
    public void LoginRemote(CustomString account, CustomString password, Proxy proxy)
    {
        string accountStr = account.Getter();
        string passwordStr = password.Getter();
        Login(accountStr, passwordStr, proxy);
    }

    /*
     * Player login and create player entity if authorized
     */
    public void Login(string account, string password, Proxy proxy)
    {
        Msg msg = new Msg("", "Gate", "LoginResRemote");
        if (!CheckAccount(account, password))
        {
            msg.arg = new CustomBool(false);
        }
        else {
            if (!account2player.ContainsKey(account))
            {
                PlayerEntity player = Game.Instance.entityManager.CreateEntity<PlayerEntity>();
                account2player[account] = player.eid.Getter();
            }
            msg.arg = new CustomBool(true);
        }
        _ = Gate.SendMsgAsync(proxy, msg);
    }
}

