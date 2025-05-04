using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerHub : MonoBehaviour
{
    // Singleton instance
    public static ManagerHub Instance { get; private set; }

    private EntityManager entityManager;
    private EventManager eventManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        entityManager = new EntityManager();
        eventManager = new EventManager();
    }

    private void OnEnable()
    {
        entityManager.Start();
        eventManager.Start();
    }

    private void OnDisable()
    {
        entityManager.Stop();
        eventManager.Stop();
    }

    #region REGION_GET_MANAGER

    public EntityManager GetEntityManager()
    {
        return entityManager;
    }

    public EventManager GetEventManager()
    {
        return eventManager;
    }

    #endregion
}
