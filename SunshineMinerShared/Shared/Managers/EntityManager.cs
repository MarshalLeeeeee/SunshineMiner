using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class EntityManager : Manager
{
    private Dictionary<string, PlayerEntity> players = new Dictionary<string, PlayerEntity>();

    #region REGION_CREATE_ENTITY

    public string CreateEntity()
    {
        Guid eid = Guid.NewGuid();
        return DoCreateEntity(eid.ToString());
    }
    public string CreateEntity(string eid)
    {
        return DoCreateEntity(eid);
    }

    private string DoCreateEntity(string eid)
    {
        if (!players.ContainsKey(eid))
        {
            PlayerEntity player = new PlayerEntity(eid);
            players.Add(eid, player);
        }
        return eid;
    }

    #endregion
}
