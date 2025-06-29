using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehavior : MonoBehaviour
{
    // Singleton instance
    public static GameBehavior Instance { get; private set; }
    private Game game;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        game = new Game();
    }

    private void OnEnable()
    {
        game.EnableManagers();
    }

    private void FixedUpdate()
    {
        game.UpdateManagers();
    }

    private void OnDisable()
    {
        game.DisableManagers();
    }
}
