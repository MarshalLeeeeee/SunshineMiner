using System;
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
