using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    // Singleton instance
    public static Game Instance { get; private set; }

    public EntityManager entityManager { get; private set; }
    public EventManager eventManager { get; private set; }

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
}
