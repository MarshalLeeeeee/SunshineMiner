using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BgTouch : Widget
{
    private Action callback;

    public void Attach(Action callback_)
    {
        callback = callback_;
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OnCick);
    }

    private void OnCick()
    {
        if (callback != null)
        {
            callback();
        }
    }
}
