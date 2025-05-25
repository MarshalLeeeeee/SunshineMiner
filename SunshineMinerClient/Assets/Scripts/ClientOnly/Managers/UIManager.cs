using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum CanvasLayer
{
    Default = 100,
    Popup = 200,
    Loading = 300
}

public class UIManager : MonoBehaviour
{
    // Singleton instance
    public static UIManager Instance { get; private set; }

    private Dictionary<CanvasLayer, Canvas> canvases = new Dictionary<CanvasLayer, Canvas>();
    private Dictionary<string, Panel> panels = new Dictionary<string, Panel>();

    [SerializeField]
    private Canvas coverCanvas; // the canvas works only at the game start stage

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadPanelAsync("PnlStart", CanvasLayer.Default);
    }

    #region REGION_PANEL

    public void LoadPanelAsync(string pnlName, CanvasLayer layer)
    {
        StartCoroutine(LoadPanelWrapper(pnlName, layer));
    }

    private IEnumerator LoadPanelWrapper(string pnlName, CanvasLayer layer)
    {
        var task = DoLoadPanelAsync(pnlName, layer);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debugger.Log($"Load panel {pnlName} failed");
        }
    }

    private async Task DoLoadPanelAsync(string pnlName, CanvasLayer layer)
    {
        string pnlPath = $"Assets/UI/Panels/{pnlName}.prefab";
        AsyncOperationHandle<GameObject> panelHandle = Addressables.LoadAssetAsync<GameObject>(pnlPath);
        await panelHandle.Task;

        if (panelHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Canvas canvas = GetCanvas(layer);
            GameObject panelGameObject = Instantiate(panelHandle.Result, canvas.transform);
            Panel panel = panelGameObject.GetComponent<Panel>();
            panel.transform.localPosition = Vector3.zero;
            panel.transform.localScale = Vector3.one;
            panels[pnlName] = panel;
        }
        else
        {
            Debug.LogError($"Load {pnlPath} failed: {panelHandle.OperationException}");
        }
    }

    public void UnloadPanel(string pnlName)
    {
        if (!panels.TryGetValue(pnlName, out var panel))
        {
            Debug.LogError($"Load panel {pnlName} not within management...");
            return;
        }
        Destroy(panel.gameObject);
        panels.Remove(pnlName);
    }

    #endregion REGION_PANEL

    private Canvas GetCanvas(CanvasLayer layer)
    {
        if (!canvases.ContainsKey(layer))
        {
            return CreateCanvas(layer);
        }
        else
        {
            return canvases[layer];
        }
    }

    private Canvas CreateCanvas(CanvasLayer layer)
    {
        GameObject canvasGO = new GameObject($"Canvas{layer}");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = (int)layer;
        canvasGO.AddComponent<GraphicRaycaster>();
        canvases[layer] = canvas;
        return canvas;
    }

    public void RemoveCover()
    {
        Destroy(coverCanvas.gameObject);
        coverCanvas = null;
    }
}
