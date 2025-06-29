using System;
using System.Collections.Generic;

public class EntityManagerCommon : Manager
{
    private Dictionary<string, PlayerEntity> players = new Dictionary<string, PlayerEntity>();

    public override void Update()
    {
        foreach (var player in players.Values)
        {
            player.Update();
        }
    }

    #region REGION_CREATE_ENTITY

    private T? CreateEntity<T>() where T : Entity
    {
        T? entity = (T?)Activator.CreateInstance(typeof(T));
        if (entity != null)
        {
            entity.Init();
            entity.OnLoad();
        }
        return entity;
    }
    private T? CreateEntity<T>(string eid) where T : Entity
    {
        T? entity = (T?)Activator.CreateInstance(typeof(T), eid);
        if (entity != null)
        {
            entity.Init();
            entity.OnLoad();
        }
        return entity;
    }
    private T? CreateEntity<T>(CustomDict baseProperty, CustomDict compProperty) where T : Entity
    {
        T? entity = (T?)Activator.CreateInstance(typeof(T));
        if (entity != null)
        {
            entity.Init(baseProperty, compProperty);
            entity.OnLoad();
        }
        return entity;
    }

    #endregion

    #region REGION_GET_ENITTY

    public Entity? GetEntity(string s)
    {
        if (players.TryGetValue(s, out var player))
        {
            return player;
        }
        else
        {
            return null;
        }
    }

    #endregion

    #region REGION_PLAYER

    public PlayerEntity? CreatePlayer(string eid)
    {
        PlayerEntity? player = CreateEntity<PlayerEntity>(eid);
        if (player != null)
        {
            players[player.eid.Getter()] = player;
            Game.Instance.eventManager.TriggerGlobalEvent("CreatePlayer", player);
        }
        return player;
    }

    public PlayerEntity? CreatePlayer(CustomDict baseProperty, CustomDict compProperty)
    {
        PlayerEntity? player = CreateEntity<PlayerEntity>(baseProperty, compProperty);
        if (player != null)
        {
            players[player.eid.Getter()] = player;
            Game.Instance.eventManager.TriggerGlobalEvent("CreatePlayer", player);
        }
        return player;
    }

    public PlayerEntity? GetPlayer(string eid)
    {
        if (players.TryGetValue(eid, out var player))
        {
            return player;
        }
        else
        {
            return null;
        }
    }

    public IEnumerable<PlayerEntity> GetOtherPlayer(string pid)
    {
        foreach (var kvp in players)
        {
            if (kvp.Key != pid)
            {
                yield return kvp.Value;
            }
        }
    }

    public IEnumerable<PlayerEntity> GetAllPlayer()
    {
        foreach (var kvp in players)
        {
            yield return kvp.Value;
        }
    }

    #endregion
}
