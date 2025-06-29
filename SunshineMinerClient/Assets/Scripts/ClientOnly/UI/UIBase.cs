using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public abstract class UIBase : MonoBehaviour
{
    protected RectTransform rectTransform;
    protected Dictionary<string, Widget> widgets = new Dictionary<string, Widget>();

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    /*
     * control visibility, logic activity
     */
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    /*
     * Get the activity
     */
    public bool GetActive()
    {
        return gameObject.activeInHierarchy;
    }

    #region REGION_WIDGET

    public void LoadWidgetAsync(string widgetName, string widgetAssetName,
        Vector2? anchoredPosition = null,
        Vector2? anchorMin = null,
        Vector2? anchorMax = null)
    {
        StartCoroutine(LoadWidgetWrapper(widgetName, widgetAssetName, anchoredPosition, anchorMin, anchorMax));
    }

    private IEnumerator LoadWidgetWrapper(string widgetName, string widgetAssetName,
        Vector2? anchoredPosition = null,
        Vector2? anchorMin = null,
        Vector2? anchorMax = null)
    {
        var task = DoLoadWidgetAsync(widgetName, widgetAssetName, anchoredPosition, anchorMin, anchorMax);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debugger.Log($"Load widget {widgetName} failed");
        }
    }

    private async Task DoLoadWidgetAsync(string widgetName, string widgetAssetName,
        Vector2? anchoredPosition = null,
        Vector2? anchorMin = null,
        Vector2? anchorMax = null)
    {
        string pnlPath = $"Assets/UI/Widgets/{widgetAssetName}.prefab";
        AsyncOperationHandle<GameObject> widgetHandle = Addressables.LoadAssetAsync<GameObject>(pnlPath);
        await widgetHandle.Task;

        if (widgetHandle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject widgetGameObject = Instantiate(widgetHandle.Result, rectTransform);
            Widget widget = widgetGameObject.GetComponent<Widget>();
            widget.transform.localPosition = Vector3.zero;
            widget.transform.localScale = Vector3.one;
            var rt = widget.GetComponent<RectTransform>();
            if (anchoredPosition.HasValue) rt.anchoredPosition = anchoredPosition.Value;
            if (anchorMin.HasValue) rt.anchorMin = anchorMin.Value;
            if (anchorMax.HasValue) rt.anchorMax = anchorMax.Value;
            widgets[widgetName] = widget;
        }
        else
        {
            Debug.LogError($"Load {pnlPath} failed: {widgetHandle.OperationException}");
        }
    }

    public void UnloadWidget(string widgetName)
    {
        if (!widgets.TryGetValue(widgetName, out var widget))
        {
            Debug.LogError($"Load widget {widgetName} not within management...");
            return;
        }
        Destroy(widget.gameObject);
        widgets.Remove(widgetName);
    }

    #endregion REGION_WIDGET
}
