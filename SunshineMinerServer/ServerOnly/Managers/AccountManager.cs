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

    /*
     * Player login and create player entity if authorized
     */
    public void ValidateAccount(string account, string password)
    {
        if (!CheckAccount(account, password)) return;

        if (!account2player.ContainsKey(account))
        {
            string playerId = Game.Instance.entityManager.CreateEntity();
            account2player[account] = playerId;
        }
    }
}

