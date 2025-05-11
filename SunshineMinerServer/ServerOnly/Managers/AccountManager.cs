using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class AccountManager : Manager
{
    private Dictionary<string, string> account2player = new Dictionary<string, string>();

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
        Msg msg = new Msg("", "", "LoginRes");
        CustomList arg = new CustomList();
        if (!CheckAccount(account, password))
        {
            arg.Add(new CustomBool(false)); ;
            msg.arg = arg;
            _ = Gate.SendMsgAsync(proxy, msg);
            return;
        }

        if (!account2player.ContainsKey(account))
        {
            string playerId = Game.Instance.entityManager.CreateEntity();
            account2player[account] = playerId;
        }
        arg.Add(new CustomBool(true)); ;
        msg.arg = arg;
        _ = Gate.SendMsgAsync(proxy, msg);
    }
}

