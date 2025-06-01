using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
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
    private bool coverLoaded = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);       
    }

    private void Start()
    {
        InitCover();
    }

    #region REGION_PANEL

    public void LoadPanelAsync(string pnlName, CanvasLayer layer)
    {
        string pnlPath = $"Assets/UI/Panels/{pnlName}.prefab";
        ResourceManager.Instance.LoadResourceAsync(pnlPath, (panelObject) => {
            OnPanelLoaded(panelObject, pnlName, layer);
        });
    }

    private void OnPanelLoaded(GameObject panelObject, string pnlName, CanvasLayer layer)
    {
        Canvas canvas = GetCanvas(layer);
        GameObject panelGameObject = Instantiate(panelObject, canvas.transform);
        Panel panel = panelGameObject.GetComponent<Panel>();
        panel.transform.localPosition = Vector3.zero;
        panel.transform.localScale = Vector3.one;
        panels[pnlName] = panel;
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

    #endregion

    #region REGION_CANVAS

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

    #endregion

    #region REGION_COVER_PANEL

    public void InitCover()
    {
        if (!coverLoaded)
        {
            LoadPanelAsync("PnlStart", CanvasLayer.Default);
            coverLoaded = true;
        }
    }

    public void RemoveCover()
    {
        Destroy(coverCanvas.gameObject);
        coverCanvas = null;
    }

    #endregion
}
