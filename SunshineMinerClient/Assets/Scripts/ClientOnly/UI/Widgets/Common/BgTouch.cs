using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BgTouch : MonoBehaviour
{
    private Action callback;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddClickCallback(Action callback_)
    {
        callback = callback_;
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OnCick);
    }

    private void OnCick()
    {
        callback();
    }
}
