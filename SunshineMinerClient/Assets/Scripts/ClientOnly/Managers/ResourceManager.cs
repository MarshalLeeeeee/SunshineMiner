using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

public class ResourceManager : MonoBehaviour
{
    // Singleton instance
    public static ResourceManager Instance { get; private set; }

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

    #region REGION_LOAD_RESOURCE

    public void LoadResourceAsync(string resourcePath, Action<GameObject> callback)
    {
        StartCoroutine(LoadResourceWrapper(resourcePath, callback));
    }

    private IEnumerator LoadResourceWrapper(string resourcePath, Action<GameObject> callback)
    {
        var task = DoLoadResourceAsync(resourcePath, callback);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debugger.Log($"Load resource {resourcePath} failed");
        }
    }

    private async Task DoLoadResourceAsync(string resourcePath, Action<GameObject> callback)
    {
        AsyncOperationHandle<GameObject> panelHandle = Addressables.LoadAssetAsync<GameObject>(resourcePath);
        await panelHandle.Task;

        if (panelHandle.Status == AsyncOperationStatus.Succeeded)
        {
            callback(panelHandle.Result);
        }
        else
        {
            Debugger.Log($"Load {resourcePath} failed: {panelHandle.OperationException}");
        }
    }

    #endregion
}
