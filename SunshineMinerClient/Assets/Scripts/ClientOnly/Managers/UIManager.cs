using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.UI;

public enum CanvasLayer
{
    Background = 0,
    Default = 100,
    Popup = 200,
    Loading = 300,
    Alert = 400,
    Debug = 1000
}


public class UIManager : MonoBehaviour
{
    // Singleton instance
    public static UIManager Instance { get; private set; }
    private Dictionary<CanvasLayer, Canvas> canvases = new Dictionary<CanvasLayer, Canvas>();
    private AsyncOperationHandle<GameObject> panelHandle;
    [SerializeField] private Canvas defaultCanvas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        canvases[CanvasLayer.Default] = defaultCanvas;
    }

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
            Debug.Log($"Load panel {pnlName} failed");
        }
    }

    public async Task DoLoadPanelAsync(string pnlName, CanvasLayer layer)
    {
        string pnlPath = $"Assets/UI/Panels/{pnlName}.prefab";
        panelHandle = Addressables.LoadAssetAsync<GameObject>(pnlPath);
        await panelHandle.Task;

        if (panelHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Canvas canvas = GetCanvas(layer);
            GameObject currentPanel = Instantiate(panelHandle.Result, canvas.transform);
            currentPanel.transform.localPosition = Vector3.zero;
            currentPanel.transform.localScale = Vector3.one;
        }
        else
        {
            Debug.LogError($"Load {pnlPath} failed: {panelHandle.OperationException}");
        }
    }

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
