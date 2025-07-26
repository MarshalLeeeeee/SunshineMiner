using System;
using System.Collections.Generic;

public class EntityManagerCommon : Manager
{
    private Dictionary<string, PlayerEntity> players = new Dictionary<string, PlayerEntity>();

    protected override void DoUpdateSelf()
    {
        foreach (var player in players.Values)
        {
            player.Update();
        }
    }

    #region REGION_ENTITY_COMMON

    private T? CreateEntity<T>() where T : Entity
    {
        T? entity = (T?)Activator.CreateInstance(typeof(T));
        if (entity != null)
        {
            entity.Init();
            entity.Enable();
        }
        return entity;
    }
    private T? CreateEntity<T>(string eid) where T : Entity
    {
        T? entity = (T?)Activator.CreateInstance(typeof(T), eid);
        if (entity != null)
        {
            entity.Init();
            entity.Enable();
        }
        return entity;
    }
    private T? CreateEntity<T>(PropDictionaryNode<string> info) where T : Entity
    {
        T? entity = (T?)Activator.CreateInstance(typeof(T));
        if (entity != null)
        {
            entity.Init(info);
            entity.Enable();
        }
        return entity;
    }
    
    private void DestroyEntity(Entity e)
    {
        e.Disable();
        e.Destroy();
    }

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
            players[player.eid.GetValue()] = player;
            Game.Instance.eventManager.TriggerGlobalEvent("CreatePlayer", player);
        }
        return player;
    }

    public PlayerEntity? CreatePlayer(PropDictionaryNode<string> info)
    {
        PlayerEntity? player = CreateEntity<PlayerEntity>(info);
        if (player != null)
        {
            players[player.eid.GetValue()] = player;
            Game.Instance.eventManager.TriggerGlobalEvent("CreatePlayer", player);
        }
        return player;
    }

    public void DestroyPlayer(string pid)
    {
        if (players.Remove(pid, out var player))
        {
            Game.Instance.eventManager.TriggerGlobalEvent("DestroyPlayer", player);
            DestroyEntity(player);
        }
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
