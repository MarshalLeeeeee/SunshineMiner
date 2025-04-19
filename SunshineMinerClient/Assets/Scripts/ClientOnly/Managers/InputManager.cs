using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputManager : MonoBehaviour
{
    // Singleton instance
    public static InputManager Instance { get; private set; }

    [Serializable]
    public class KeyBinding
    {
        public string actionName;
        public KeyCode defaultKey;
        [NonSerialized] public KeyCode currentKey;
    }

    [SerializeField] private List<KeyBinding> keyBindings = new List<KeyBinding>();

    public Dictionary<string, KeyCode> actionToKey = new Dictionary<string, KeyCode>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _LoadKeyBindings();
    }

    private void _LoadKeyBindings()
    {
        foreach (var binding in keyBindings)
        {
            if (PlayerPrefs.HasKey(binding.actionName))
            {
                binding.currentKey = (KeyCode)PlayerPrefs.GetInt(binding.actionName);
            }
            else
            {
                binding.currentKey = binding.defaultKey;
            }
            actionToKey[binding.actionName] = binding.currentKey;
        }

    }

    public bool GetAction(string actionName)
    {
        if (actionToKey.ContainsKey(actionName))
        {
            return Input.GetKey(actionToKey[actionName]);
        }
        else
        {
            return false;
        }
    }

    public bool GetActionDown(string actionName)
    {
        if (actionToKey.ContainsKey(actionName))
        {
            return Input.GetKeyDown(actionToKey[actionName]);
        }
        else
        {
            return false;
        }
    }

    public bool GetActionUp(string actionName)
    {
        if (actionToKey.ContainsKey(actionName))
        {
            return Input.GetKeyUp(actionToKey[actionName]);
        }
        else
        {
            return false;
        }
    }
}
